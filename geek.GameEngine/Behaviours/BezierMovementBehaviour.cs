using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// Lets an object move along a bezier curve.
	/// </summary>
	public class BezierMovementBehaviour : IBehaviour
	{
		#region Constructor

		public BezierMovementBehaviour(params Vector2[] points)
			: this(points.ToList())
		{ }

		public BezierMovementBehaviour(IEnumerable<Vector2> points)
		{
			_Points = points.ToList();

			if (_Points.Count < 3 || _Points.Count % 2 == 0)
				throw new ArgumentException("The bezier curve requires an odd number of points (at least 3).");

			reset();
		}

		#endregion

		#region Fields

		/// <summary>
		/// The flag indicating the object has been placed into the curve's initial location.
		/// </summary>
		private bool _IsActivated;

		/// <summary>
		/// The flag indicating the movement has finished.
		/// </summary>
		public bool IsFinished { get; private set; }

		/// <summary>
		/// The binding to a pause.
		/// </summary>
		public Func<bool> PauseBinding { get; set; }

		/// <summary>
		/// The points of the curve.
		/// </summary>
		private readonly List<Vector2> _Points;

		/// <summary>
		/// The id of the first point of the current segment.
		/// </summary>
		private int _CurrentPoint;

		/// <summary>
		/// The current segment.
		/// </summary>
		private BezierSegment _Segment;

		#endregion

		#region Methods

		public void UpdateObjectState(DynamicObject obj)
		{
			if (!_IsActivated)
			{
				obj.Position = _Points[0];
				_IsActivated = true;
				_Segment = new BezierSegment(_Points[0], _Points[1], _Points[2]);
			}

			if (IsFinished || (PauseBinding != null && PauseBinding()) || GameCore.Delta.IsAlmostNull())
				return;

			// calculate the desired momentum
			var speed = obj.Speed;
			var oldPoint = _Segment.CurrentPoint;
			_Segment.Step(speed*GameCore.Delta);
			var newPoint = _Segment.CurrentPoint;
			var vector = newPoint - oldPoint;
			if (vector.Length() > 0)
			{
				vector.Normalize();
				obj.Momentum = vector*speed;
			}
			else
			{
				obj.Momentum = Vector2.Zero;
			}

			// advance to the next segment if needed
			if (_Segment.IsFinished)
			{
				if (_CurrentPoint + 4 > _Points.Count)
				{
					IsFinished = true;
					GameCore.RegisterDeferredAction(() =>
					    {
					        obj.Momentum = Vector2.Zero;
							obj.Behaviours.Remove<BezierMovementBehaviour>();
					    }
					);
					
					return;
				}

				_CurrentPoint += 2;
				_Segment = new BezierSegment(_Points[_CurrentPoint], _Points[_CurrentPoint + 1], _Points[_CurrentPoint+2]);
			}
		}

		/// <summary>
		/// Reverse the movement along the curve.
		/// </summary>
		public void Reverse()
		{
			reset();
			_Points.Reverse();
		}

		/// <summary>
		/// Reinitialize the properties of the bezier curve path.
		/// </summary>
		private void reset()
		{
			_IsActivated = false;
			IsFinished = false;
			_CurrentPoint = 0;
		}

		#endregion
	}
}
