using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Scrolling
{
	/// <summary>
	/// The scroll manager that points the screen to a particular object.
	/// </summary>
	public class FollowScroll : IScrollManager
	{
		#region Constructors

		public FollowScroll(VisualObjectBase target)
		{
			Target = target;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets the target to which the scrolling manager points.
		/// </summary>
		public VisualObjectBase Target;

		#endregion

		#region Methods

		public Vector2 UpdateScrolling(Vector2 lastPosition)
		{
			return Target.AbsolutePosition - GameCore.ScreenSize / 2;
		}

		#endregion
	}
}
