using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Sprites
{
	/// <summary>
	/// Basic class for a non-animated sprite.
	/// </summary>
	public class Sprite : SpriteBase
	{
		#region Constructors

		public Sprite(GameObject parent, Texture2D tex)
			: base(parent, tex)
		{
			Width = Texture.Width;
			Height = Texture.Height;
			Size = new Vector2(Width, Height);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets or sets the hot spot in the sprite, around which it is centered, rotated and scaled.
		/// </summary>
		public override Vector2 HotSpot { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Display the sprite to the screen.
		/// </summary>
		/// <param name="batch">Sprite batch.</param>
		/// <param name="disableLayering">Whether to use layer ordering or not.</param>
		public override void Draw(SpriteBatch batch, bool disableLayering = false)
		{
			if(Parent.ScaleVector.HasValue)
				batch.Draw(
					Texture,
					Parent.RelativePosition,
					null,
					Parent.TintColor,
					Parent.Angle,
					HotSpot,
					Parent.ScaleVector.Value,
					SpriteEffects.None,
					disableLayering ? 0 : VisualObjectBase.LayerId
				);
			else
				batch.Draw(
					Texture,
					Parent.RelativePosition,
					null,
					Parent.TintColor,
					Parent.Angle,
					HotSpot,
					Parent.Scale,
					SpriteEffects.None,
					disableLayering ? 0 : VisualObjectBase.LayerId
				);
		}

		/// <summary>
		/// Get a portion of the current frame's texture as a sequence of colors.
		/// </summary>
		/// <param name="rect">Rectangle to retrieve.</param>
		/// <returns></returns>
		public override Color[] GetTextureRegion(Rectangle rect)
		{
			var result = new Color[rect.Width * rect.Height];
			Texture.GetData(0, rect, result, 0, result.Length);
			return result;
		}

		#endregion
	}
}
