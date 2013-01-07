using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Sprites
{
	/// <summary>
	/// Makes a sprite blended with given color.
	/// http://gamedev.stackexchange.com/questions/24825/xna-sprite-flash-effect
	/// </summary>
	public class BlendableSprite : SpriteBase
	{
		#region Constructors

		static BlendableSprite()
		{
			_StencilBefore = new DepthStencilState
			{
				StencilEnable = true,
				StencilFunction = CompareFunction.Always,
				StencilPass = StencilOperation.Replace,
				ReferenceStencil = 1
			};

			_StencilAfter = new DepthStencilState
			{
				StencilEnable = true,
				StencilFunction = CompareFunction.Equal,
				ReferenceStencil = 1
			};
		}
		
		public BlendableSprite(GameObject parent, SpriteBase sprite, Color color)
			: base(parent)
		{
			_Sprite = sprite;

			Width = _Sprite.Width;
			Height = _Sprite.Height;
			Size = _Sprite.Size;

			_BlendTexture = new Texture2D(GameCore.GraphicsDevice, 1, 1);
			_BlendTexture.SetData(new[] { color });

			_AlphaTestEffect = new AlphaTestEffect(GameCore.GraphicsDevice)
			{
				DiffuseColor = Color.White.ToVector3(),
				AlphaFunction = CompareFunction.Greater,
				ReferenceAlpha = 0,
				World = Matrix.Identity,
				View = Matrix.Identity,
				Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
					Matrix.CreateOrthographicOffCenter(0, GameCore.GraphicsDevice.Viewport.Width, GameCore.GraphicsDevice.Viewport.Height, 0, 0, 1)
			};
		}

		#endregion

		#region Fields

		public override Vector2 HotSpot
		{
			get { return _Sprite.HotSpot; }
			set { _Sprite.HotSpot = value; }
		}

		/// <summary>
		/// The color to blend to.
		/// </summary>
		public Color Color
		{
			get
			{
				var colorData = new Color[1];
				_BlendTexture.GetData(colorData);
				return colorData[0];
			}
			set
			{
				_BlendTexture.SetData(new [] { value });
			}
		}

		/// <summary>
		/// Texture to blend the sprite with.
		/// </summary>
		private readonly Texture2D _BlendTexture;

		/// <summary>
		/// Sprite to wrap into.
		/// </summary>
		private readonly SpriteBase _Sprite;

		/// <summary>
		/// Alpha test effect that detects blendable pixels.
		/// </summary>
		private readonly AlphaTestEffect _AlphaTestEffect;

		/// <summary>
		/// Usual stencil.
		/// </summary>
		private static readonly DepthStencilState _StencilBefore;

		/// <summary>
		/// Color-blended stencil.
		/// </summary>
		private static readonly DepthStencilState _StencilAfter;

		#endregion

		#region Methods

		public override void Reset()
		{
			_Sprite.Reset();
		}

		public override void Update()
		{
			_Sprite.Update();
		}

		public override Color[] GetTextureRegion(Rectangle rect)
		{
			return _Sprite.GetTextureRegion(rect);
		}

		/// <summary>
		/// Draw the texture into stencil buffer, and then use the stencil to draw a single-colored texture.
		/// </summary>
		/// <param name="batch">Sprite batch</param>
		/// <param name="disableLayering">Whether to use layer ordering or not.</param>
		public override void Draw(SpriteBatch batch, bool disableLayering = false)
		{
			if (!Parent.IsBlendingEnabled)
			{
				_Sprite.Draw(batch);
				return;
			}

			// clear the stencil buffer
			GameCore.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);

			// draw the texture using alpha-test into stencil buffer
			var newBatch = new SpriteBatch(GameCore.GraphicsDevice);

			newBatch.Begin(SpriteSortMode.Deferred, null, null, _StencilBefore, null, _AlphaTestEffect);
			_Sprite.Draw(newBatch, true);
			newBatch.End();

			// draw the whole single-colored pixel into the screen, using the stencil buffer
			newBatch.Begin(SpriteSortMode.Deferred, null, null, _StencilAfter, null);
			newBatch.Draw(_BlendTexture, GameCore.GraphicsDevice.Viewport.Bounds, Color.White);
			newBatch.End();
		}

		#endregion
	}
}
