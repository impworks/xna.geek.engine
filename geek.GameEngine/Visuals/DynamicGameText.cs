using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace geek.GameEngine.Visuals
{
	public class DynamicGameText : GameText
	{
		#region Constructor

		public DynamicGameText(SpriteFont font, string text, float speed)
			: base(font, text)
		{
			_CharacterDelay = 1 / speed;
			Reset();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets the size of the current string displayable.
		/// </summary>
		public override Vector2 Size
		{
			get { return Font.MeasureString(_TextBuffer); }
		}

		/// <summary>
		/// Gets the flag indicating the text animation has finished.
		/// </summary>
		public bool Finished
		{
			get { return _CurrentCharacter == Text.Length; }
		}

		/// <summary>
		/// The time between delay between characters appearing.
		/// </summary>
		private readonly float _CharacterDelay;

		/// <summary>
		/// The buffer to store current text portion.
		/// </summary>
		private StringBuilder _TextBuffer;

		/// <summary>
		/// The current character index.
		/// </summary>
		private int _CurrentCharacter;

		/// <summary>
		/// The amount of time elapsed since last character appearance.
		/// </summary>
		private float _ElapsedCharacterTime;

		#endregion

		#region VisualObjectBase overrides

		public override void Update()
		{
			base.Update();

			if(Finished)
				return;

			_ElapsedCharacterTime += GameCore.Delta;
			if(_ElapsedCharacterTime > _CharacterDelay)
			{
				_TextBuffer.Append(Text[_CurrentCharacter]);
				_CurrentCharacter++;
				_ElapsedCharacterTime -= _CharacterDelay;
				resetDimensions();
			}
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.DrawString(
				Font,
				_TextBuffer,
				Position,
				TintColor,
				Angle,
				_HotSpot,
				Scale,
				SpriteEffects.None,
				1f
			);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Resets the text animation to it's start.
		/// </summary>
		public void Reset()
		{
			_TextBuffer = new StringBuilder(Text.Length);
			_CurrentCharacter = 0;
			_ElapsedCharacterTime = 0;
		}

		/// <summary>
		/// Skips the animation and displays the text completely.
		/// </summary>
		public void SkipAnimation()
		{
			_TextBuffer.Append(Text, _CurrentCharacter, Text.Length - _CurrentCharacter);
			_CurrentCharacter = Text.Length;
			resetDimensions();
		}

		#endregion
	}
}
