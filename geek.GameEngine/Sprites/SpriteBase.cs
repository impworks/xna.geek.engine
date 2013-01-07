using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Sprites
{
	public abstract class SpriteBase
	{
		#region Constructors

		protected SpriteBase(GameObject parent)
		{
			Parent = parent;
		}

		protected SpriteBase(GameObject parent, Texture2D tex)
		{
			Texture = tex;
			Parent = parent;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets the texture behind the sprite.
		/// </summary>
		public Texture2D Texture { get; protected set; }

		/// <summary>
		/// Gets the parent game object.
		/// </summary>
		public GameObject Parent { get; protected set; }

		/// <summary>
		/// Gets the size of the sprite as a 2D vector.
		/// </summary>
		public Vector2 Size { get; protected set; }

		/// <summary>
		/// Gets the width of the sprite.
		/// </summary>
		public int Width { get; protected set; }

		/// <summary>
		/// Gets the height of the sprite.
		/// </summary>
		public int Height { get; protected set; }

		/// <summary>
		/// Gets or sets the hotspot for current frame.
		/// </summary>
		public abstract Vector2 HotSpot { get; set; }

		/// <summary>
		/// Get a piece of sprite texture for collision detection.
		/// </summary>
		/// <param name="rect">Rectangle to get.</param>
		/// <returns></returns>
		public abstract Color[] GetTextureRegion(Rectangle rect);

		#endregion

		#region Methods

		/// <summary>
		/// Update the sprite.
		/// </summary>
		public virtual void Update()
		{ }

		/// <summary>
		/// Reset the sprite.
		/// </summary>
		public virtual void Reset()
		{ }

		/// <summary>
		/// Outputs the sprite to the screen.
		/// </summary>
		/// <param name="batch">Sprite batch.</param>
		/// <param name="disableLayering">Whether to use layer ordering or not.</param>
		public abstract void Draw(SpriteBatch batch, bool disableLayering = false);

		/// <summary>
		/// Set the hotspot to a particular place inside the sprite.
		/// </summary>
		/// <param name="horizontal"></param>
		/// <param name="vertical"></param>
		public void SetHotSpot(HorizontalAlignment horizontal = HorizontalAlignment.Center, VerticalAlignment vertical = VerticalAlignment.Center)
		{
			var x = Width;
			var y = Height;

			switch (horizontal)
			{
				case HorizontalAlignment.Left: x = 0; break;
				case HorizontalAlignment.Center: x /= 2; break;
			}

			switch (vertical)
			{
				case VerticalAlignment.Top: y = 0; break;
				case VerticalAlignment.Center: y /= 2; break;
			}

			HotSpot = new Vector2(x, y);
		}

		#endregion
	}
}
