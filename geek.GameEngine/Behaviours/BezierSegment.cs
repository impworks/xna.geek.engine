using System;
using Microsoft.Xna.Framework;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// A class representing a segment of the bezier curve along which an object moves.
	/// </summary>
	internal class BezierSegment
	{
		#region Constructor

		public BezierSegment(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Point1 = p1;
			Point2 = p2;
			Point3 = p3;
			CurrentPoint = p1;
			calculateLength();
		}

		#endregion

		#region Fields

		/// <summary>
		/// The flag indicating the current segment is finished.
		/// </summary>
		public bool IsFinished { get; private set; }

		/// <summary>
		/// The current point in the segment.
		/// </summary>
		public Vector2 CurrentPoint { get; private set; }

		/// <summary>
		/// The length of the 
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		/// The first point of the arc.
		/// </summary>
		private readonly Vector2 Point1;

		/// <summary>
		/// The second point of the arc.
		/// </summary>
		private readonly Vector2 Point2;

		/// <summary>
		/// The third point of the arc.
		/// </summary>
		private readonly Vector2 Point3;

		/// <summary>
		/// The length of the path the object has passed already.
		/// </summary>
		private float _Elapsed;

		#endregion

		#region Methods

		/// <summary>
		/// Advance the object along the curve.
		/// </summary>
		/// <param name="length"></param>
		public void Step(float length)
		{
			if(IsFinished)
				return;

			_Elapsed += length;
			if (_Elapsed >= Length)
			{
				_Elapsed = Length;
				IsFinished = true;
			}

			if(!Length.IsAlmostNull())
				updatePoint(_Elapsed / Length);
		}

		/// <summary>
		/// Get the length of the curve.
		/// http://segfaultlabs.com/docs/quadratic-bezier-curve-length
		/// </summary>
		private void calculateLength()
		{
			var aX = Point1.X - 2*Point2.X + Point3.X;
			var aY = Point1.Y - 2*Point2.Y + Point3.Y;
			var bX = 2*(Point2.X - Point1.X);
			var bY = 2*(Point2.Y - Point1.Y);

			var A = 4*(aX*aX + aY*aY);
			var B = 4*(aX*bX + aY*bY);
			var C = bX*bX + bY*bY;

			var Sabc = (float)(2*Math.Sqrt(A + B + C));
			var A_2 = (float)Math.Sqrt(A);
			var A_32 = 2*A*A_2;
			var C_2 = (float)(2*Math.Sqrt(C));
			var BA = B/A_2;

			if((BA + C_2).IsAlmostNull())
				Length = (Point2 - Point1).Length() + (Point3 - Point2).Length();
			else
				Length = (A_32*Sabc + A_2*B*(Sabc - C_2) + (4*C*A - B*B)*(float) Math.Log((2*A_2 + BA + Sabc)/(BA + C_2)))/(4*A_32);
		}

		/// <summary>
		/// Update the current point.
		/// </summary>
		/// <param name="percent">The progress coefficient.</param>
		private void updatePoint(float percent)
		{
			var p1 = getPoint(Point1, Point2, percent);
			var p2 = getPoint(Point2, Point3, percent);
			CurrentPoint = getPoint(p1, p2, percent);

			if(float.IsNaN(CurrentPoint.X))
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Find a point on the line piece.
		/// </summary>
		/// <param name="p1">Line start.</param>
		/// <param name="p2">Line end.</param>
		/// <param name="percent">The point's position on the line piece.</param>
		/// <returns></returns>
		private Vector2 getPoint(Vector2 p1, Vector2 p2, float percent)
		{
			return new Vector2(
				p1.X + (p2.X - p1.X) * percent,
				p1.Y + (p2.Y - p1.Y) * percent
			);
		}

		#endregion
	}
}
