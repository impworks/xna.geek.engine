using Microsoft.Xna.Framework;

namespace geek.GameEngine.Scrolling
{
	/// <summary>
	/// No scrolling is performed.
	/// </summary>
	public class NoScroll : IScrollManager
	{
		/// <summary>
		/// The global NoScroll instance.
		/// </summary>
		public static NoScroll Instance = new NoScroll();

		/// <summary>
		/// Private constructor ensures that NoScroll is a singleton.
		/// </summary>
		private NoScroll()
		{}

		public Vector2 UpdateScrolling(Vector2 lastValue)
		{
			return lastValue;
		}
	}
}
