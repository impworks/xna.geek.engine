using geek.GameEngine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// An object that is used as a stub for real graphic objects (used to prototype games).
	/// </summary>
	public class FauxObject : GameObject
	{
		#region Constructor
		
		public FauxObject(int width, int height, FauxObjectType type = FauxObjectType.Rectangle, Color? color = null)
		{
			Type = type;

			if (!color.HasValue)
				color = new Color(Globals.Random(0, 255), Globals.Random(0, 255), Globals.Random(0, 255));

			if (type == FauxObjectType.Ellipse)
			{
				var tex = new Texture2D(GameCore.GraphicsDevice, width, height);
				tex.SetData(createEllipse(width, height, color.Value));
				Animations.Add(new Sprite(this, tex));
			}
			else
			{
				ScaleVector = new Vector2(width, height);
				var tex = new Texture2D(GameCore.GraphicsDevice, 1, 1);
				tex.SetData(new [] { color.Value });
				Animations.Add(new Sprite(this, tex));
				CollisionDetectionMode = CollisionDetectionMode.Fast;
			}
		}

		#endregion

		#region Fields

		/// <summary>
		/// The type of the faux object.
		/// </summary>
		public readonly FauxObjectType Type;

		#endregion

		#region Methods

		/// <summary>
		/// Create a faux ellipse texture.
		/// </summary>
		/// <param name="width">Ellipse width.</param>
		/// <param name="height">Ellipse height.</param>
		/// <param name="color">Texture color.</param>
		/// <returns></returns>
		private static Color[] createEllipse(int width, int height, Color color)
		{
			var centerX = width / 2f;
			var centerY = width / 2f;
			var data = new Color[width * height];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var distX = Math.Abs(centerX - x);
					var distY = Math.Abs(centerY - y);
					var contains = (distX * distX) / (centerX * centerX) + (distY * distY) / (centerY * centerY) < 1.0;
					data[y * width + x] = contains ? color : Color.Transparent;
				}
			}
			return data;
		}

		#endregion
	}

	/// <summary>
	/// The type of the faux object's shape.
	/// </summary>
	public enum FauxObjectType
	{
		Rectangle,
		Ellipse,
	}
}
