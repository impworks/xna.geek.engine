using Microsoft.Xna.Framework;

namespace geek.GameEngine.Scrolling
{
	/// <summary>
	/// The base interface for all scrolling kinds.
	/// </summary>
	public interface IScrollManager
	{
		/// <summary>
		/// Update scrolling vector.
		/// </summary>
		/// <param name="lastValue">Last scrolling offset.</param>
		/// <returns></returns>
		Vector2 UpdateScrolling(Vector2 lastValue);
	}

	/// <summary>
	/// The base interface for all scrolling kinds that automatically update their state
	/// (Like, ease-in scrolling to a point).
	/// </summary>
	public interface IAutoScrollManager : IScrollManager
	{
		/// <summary>
		/// Gets the flag indicating the scroll has finished.
		/// </summary>
		bool Finished { get; }
	}
}
