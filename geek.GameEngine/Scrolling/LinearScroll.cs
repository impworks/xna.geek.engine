using Microsoft.Xna.Framework;
using geek.GameEngine.AnimatedProperties;

namespace geek.GameEngine.Scrolling
{
	public class LinearScroll : IAutoScrollManager
	{
		#region Constructors

		public LinearScroll(Vector2 position, Vector2 target, float duration, InterpolationMode mode = InterpolationMode.Linear)
		{
			_InitialPoint = position;
			_TargetPoint = target;
			_Method = Interpolate.GetMethod(mode);
			_Duration = duration;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The initial scrolling point.
		/// </summary>
		private readonly Vector2 _InitialPoint;

		/// <summary>
		/// The current point to which the scroll manager is scrolled.
		/// </summary>
		private Vector2 _CurrentPoint;

		/// <summary>
		/// The target point to reach.
		/// </summary>
		private readonly Vector2 _TargetPoint;

		/// <summary>
		/// Interpolation method to use.
		/// </summary>
		private readonly InterpolationMethod _Method;

		/// <summary>
		/// The scrolling duration.
		/// </summary>
		private readonly float _Duration;

		/// <summary>
		/// The currently elapsed time since scrolling started.
		/// </summary>
		private float _ElapsedTime;

		/// <summary>
		/// Flag that indicates that the scroll has finished.
		/// </summary>
		public bool Finished { get { return (_CurrentPoint - _TargetPoint).Length().IsAlmostNull(); } }

		#endregion

		public Vector2 UpdateScrolling(Vector2 lastValue)
		{
			_ElapsedTime += GameCore.Delta;
			var tweenState = _ElapsedTime/_Duration;

			_CurrentPoint = new Vector2(
				_Method(_InitialPoint.X, _TargetPoint.X, tweenState),
				_Method(_InitialPoint.Y, _TargetPoint.Y, tweenState)
			);

			return _CurrentPoint;
		}
	}
}
