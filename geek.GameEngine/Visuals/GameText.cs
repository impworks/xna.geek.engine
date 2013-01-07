using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// The class representing the text drawable on screen.
	/// </summary>
	public class GameText : InteractableObject
	{
		#region Constructors

		public GameText(SpriteFont font, string text = "")
		{
			Font = font;
			_Text = text;
		}

		public GameText(string fontAsset, string text = "")
		{
			Font = ResourceCache.Get<SpriteFont>(fontAsset);
			_Text = text;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets or sets the font of the text.
		/// </summary>
		public SpriteFont Font;

		/// <summary>
		/// Gets or sets the string to output.
		/// </summary>
		public string Text
		{
			get { return _Text; }
			set
			{
				_Text = value;
				resetDimensions();
			}
		}
		private string _Text;

		/// <summary>
		/// Gets or sets the horizontal text alignment.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment
		{
			get { return _HorizontalAlignment; }
			set
			{
				_HorizontalAlignment = value;
				resetDimensions();
			}
		}
		private HorizontalAlignment _HorizontalAlignment;

		/// <summary>
		/// Gets or sets the horizontal text alignment.
		/// </summary>
		public VerticalAlignment VerticalAlignment
		{
			get { return _VerticalAlignment; }
			set
			{
				_VerticalAlignment = value;
				resetDimensions();
			}
		}
		private VerticalAlignment _VerticalAlignment;

		/// <summary>
		/// Gets the dimensions of the text block using current font.
		/// </summary>
		public virtual Vector2 Size
		{
			get { return Font.MeasureString(_Text); }
		}

		/// <summary>
		/// The maximum width of the text. The text is auto-resized to fit it.
		/// </summary>
		public int? MaxWidth
		{
			get { return _MaxWidth; }
			set
			{
				_MaxWidth = value;
				resetDimensions();
			}
		}
		private int? _MaxWidth;

		/// <summary>
		/// The text hotspot defined by text alignment.
		/// </summary>
		protected Vector2 _HotSpot;

		#endregion

		#region Methods

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			batch.DrawString(
				Font,
				_Text,
				AbsolutePosition,
				TintColor,
				Angle,
				_HotSpot,
				Scale,
				SpriteEffects.None,
				LayerId
			);
		}

		/// <summary>
		/// Recalculate hot spot and scaling.
		/// </summary>
		protected void resetDimensions()
		{
			var size = Font.MeasureString(Text);
			var width = size.X;
			var height = size.Y;

			Scale = _MaxWidth.HasValue && width > _MaxWidth.Value ? _MaxWidth.Value / width : 1;
			
			switch (HorizontalAlignment)
			{
				case HorizontalAlignment.Left:		width = 0;		break;
				case HorizontalAlignment.Center:	width /= 2;		break;
			}

			switch (VerticalAlignment)
			{
				case VerticalAlignment.Top:			height = 0;		break;
				case VerticalAlignment.Center:		height /= 2;	break;
			}

			_HotSpot = new Vector2(width, height);
		}

		public override Rectangle GetBoundingBox(bool absolute = false)
		{
			var pos = GetPosition(absolute);
			var topLeft = pos - _HotSpot * Scale;
			var size = Size;

			return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(size.X * Scale), (int)(size.Y * Scale));
		}

		#endregion
	}

	/// <summary>
	/// The horizontal text alignment modes.
	/// </summary>
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	/// <summary>
	/// The verticaL text alignment modes.
	/// </summary>
	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}
}
