using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.Visuals;


namespace geek.GameEngine.Sprites
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class AnimatedSprite : SpriteBase
	{
		#region Constructors

		public AnimatedSprite(GameObject parent, Texture2D tex, int frameCount, float frameRate)
			: base(parent, tex)
		{
			FrameCount = frameCount;
			var width = tex.Width;
			if (width % frameCount != 0)
				throw new ArgumentOutOfRangeException("The texture does not contain a whole number of frames!");

			Width = width / frameCount;
			Height = Texture.Height;
			Size = new Vector2(Width, Height);

			FrameDelay = 1/frameRate;
			_HotSpots = new Vector2[FrameCount];

			_CurrentFrameBounds = new Rectangle(0, 0, Width, Texture.Height);

			IsPlaying = true;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The flag indicating whether animation should loop after it has finished,
		/// or display the last frame until it's changed.
		/// </summary>
		public bool Looped;

		/// <summary>
		/// The time delta between frame changes.
		/// </summary>
		public float FrameDelay { get; set; }

		/// <summary>
		/// The number of frames in the animation.
		/// </summary>
		public int FrameCount { get; protected set; }

		/// <summary>
		/// Gets or sets the animation's common hotspot.
		/// </summary>
		public override Vector2 HotSpot
		{
			get { return _HotSpots[0]; }
			set
			{
				for (var idx = 0; idx < _HotSpots.Length; idx++)
					_HotSpots[idx] = value;
			}
		}

		private Vector2[] _HotSpots;
		/// <summary>
		/// The array of hotspots for animation frames.
		/// </summary>
		public Vector2[] HotSpots
		{
			get{ return _HotSpots; }
			set
			{
				if(value.Length != FrameCount)
					throw new ArgumentException(string.Format("Animation contains {0} frames, {1} hotspots given instead!", FrameCount, value.Length));

				_HotSpots = value;
			}
		}

		/// <summary>
		/// Sets the hotspot for the current frame.
		/// </summary>
		public Vector2 FrameHotSpot
		{
			get { return _HotSpots[_CurrentFrame]; }
			set { _HotSpots[_CurrentFrame] = value; }
		}

		/// <summary>
		/// The length of the animation in seconds.
		/// </summary>
		public float Duration
		{
			get { return FrameCount*FrameDelay; }
		}

		/// <summary>
		/// The flag indicating whether the animation has finished playing.
		/// </summary>
		public bool Finished
		{
			get { return !Looped && _CurrentFrame == FrameCount - 1; }
		}

		/// <summary>
		/// The current frame index;
		/// </summary>
		private int _CurrentFrame;

		/// <summary>
		/// The bounds of current frame.
		/// </summary>
		private Rectangle _CurrentFrameBounds;

		/// <summary>
		/// The amount of time elapsed since last frame change.
		/// </summary>
		private float _ElapsedFrameTime;

		/// <summary>
		/// Checks if the animation should play.
		/// </summary>
		public bool IsPlaying { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Start the animation.
		/// </summary>
		public void Start()
		{
			IsPlaying = true;
		}

		/// <summary>
		/// Pauses the animation at current frame.
		/// </summary>
		public void Pause()
		{
			IsPlaying = false;
		}

		/// <summary>
		/// Stops the animation and rewinds it to the first frame.
		/// </summary>
		public void Stop()
		{
			IsPlaying = false;
			SetFrame(0);
		}

		/// <summary>
		/// Sets the current frame.
		/// </summary>
		/// <param name="frame">Frame id (0-based).</param>
		public void SetFrame(int frame)
		{
			_CurrentFrame = 0;
			_CurrentFrameBounds = new Rectangle(Width * frame, 0, Width, Texture.Height);
		}

		/// <summary>
		/// Update the sprite.
		/// </summary>
		public override void Update()
		{
			if (!IsPlaying)
				return;

			_ElapsedFrameTime += GameCore.Delta;
			if(_ElapsedFrameTime > FrameDelay)
			{
				if (_CurrentFrame < FrameCount - 1)
				{
					_CurrentFrame++;
				}
				else
				{
					if (Looped)
						_CurrentFrame = 0;
				}

				_CurrentFrameBounds = new Rectangle(_CurrentFrame * Width, 0, Width, Texture.Height);
				_ElapsedFrameTime -= FrameDelay;
			}
		}

		/// <summary>
		/// Reset the sprite.
		/// </summary>
		public override void Reset()
		{
			SetFrame(0);
			_ElapsedFrameTime = 0;
		}

		/// <summary>
		/// Draws the current sprite frame to the screen.
		/// </summary>
		/// <param name="batch">Sprite batch.</param>
		/// <param name="disableLayering">Whether to use layer ordering or not.</param>
		public override void Draw(SpriteBatch batch, bool disableLayering = false)
		{
			if(Parent.ScaleVector.HasValue)
				batch.Draw(
					Texture,
					Parent.RelativePosition,
					_CurrentFrameBounds,
					Parent.TintColor,
					Parent.Angle,
					HotSpots[_CurrentFrame],
					Parent.ScaleVector.Value,
					SpriteEffects.None,
					disableLayering ? 0 : VisualObjectBase.LayerId
				);
			else
				batch.Draw(
					Texture,
					Parent.RelativePosition,
					_CurrentFrameBounds,
					Parent.TintColor,
					Parent.Angle,
					HotSpots[_CurrentFrame],
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
			rect.X += _CurrentFrame*Width;
			var result = new Color[rect.Width * rect.Height];
			Texture.GetData(0, rect, result, 0, result.Length);
			return result;
		}

		#endregion
	}
}
