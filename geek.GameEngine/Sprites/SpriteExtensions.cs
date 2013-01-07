using Microsoft.Xna.Framework;

namespace geek.GameEngine.Sprites
{
	/// <summary>
	/// Sprite extension methods.
	/// </summary>
	public static class SpriteExtensions
	{
		/// <summary>
		/// Make the sprite blendable.
		/// </summary>
		/// <param name="sprite">Sprite to blend.</param>
		/// <param name="color">Color to blend with.</param>
		/// <returns></returns>
		public static SpriteBase MakeBlendable(this SpriteBase sprite, Color color)
		{
			var anims = sprite.Parent.Animations;
			anims.Remove(sprite);

			var newSprite = new BlendableSprite(sprite.Parent, sprite, color);
			anims.Add(newSprite);
			return newSprite;
		}
	}
}
