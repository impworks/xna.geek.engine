using System;
using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The gravity behaviour: applies a directional force to the object.
	/// </summary>
	public class GravityBehaviour : IBehaviour
	{
		private const int DefaultSpeedLimit = 200;

		#region Constructors
		public GravityBehaviour(float coefficient, float limit = DefaultSpeedLimit)
			: this(coefficient, GravityDirection.Down, limit)
		{}

		public GravityBehaviour(float coefficient, GravityDirection direction, float limit = DefaultSpeedLimit)
		{
			_GravityVector = Globals.CreateVector(coefficient, direction.ToAngle());
			SpeedLimit = limit;
		}
		#endregion

		#region Fields

		/// <summary>
		/// Gravity vector to apply to object's momentum per pass.
		/// </summary>
		private readonly Vector2 _GravityVector;

		/// <summary>
		/// The maximum speed the object can achieve using gravity behaviour.
		/// </summary>
		public float SpeedLimit;

		#endregion

		public void UpdateObjectState(DynamicObject obj)
		{
			var result = obj.Momentum + _GravityVector * GameCore.Delta;
			var projection = _GravityVector.X.IsAlmostNull() ? _GravityVector.X : _GravityVector.Y;
			if (Math.Abs(projection) <= SpeedLimit)
				obj.Momentum = result;
		}
	}

	/// <summary>
	/// The basic set of gravity directions.
	/// </summary>
	public enum GravityDirection
	{
		Up,
		Down,
		Left,
		Right
	}

	public static class GravityDirectionExtensions
	{
		/// <summary>
		/// Convert GravityDirection to angle.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static float ToAngle(this GravityDirection direction)
		{
			switch (direction)
			{
				case GravityDirection.Up: return MathHelper.Pi * 1.5f;
				case GravityDirection.Left: return MathHelper.Pi;
				case GravityDirection.Down: return MathHelper.PiOver2;
				default: return 0;
			}
		}
	}
}
