using System;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The behaviour that deceleates object's momentum over time.
	/// </summary>
	public class FrictionBehaviour : IBehaviour
	{
		#region Constructor

		/// <summary>
		/// Create a new friction behaviour instance.
		/// </summary>
		/// <param name="coeff">
		/// Friction coefficient.
		/// 0 = no friction.
		/// 1 = stop within 1 second.
		/// >60 = stop immediately.
		/// </param>
		/// <param name="direction">Friction direction.</param>
		public FrictionBehaviour(float coeff, FrictionDirection direction = FrictionDirection.Both)
		{
			_FrictionCoefficient = coeff;
			_Direction = direction;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The friction coefficient.
		/// </summary>
		private readonly float _FrictionCoefficient;

		/// <summary>
		/// The friction direction;
		/// </summary>
		private readonly FrictionDirection _Direction;

		#endregion

		#region Methods

		public void UpdateObjectState(DynamicObject obj)
		{
			var coeff = (1 - _FrictionCoefficient*GameCore.Delta);

			if((_Direction & FrictionDirection.Horizontal) != 0)
				obj.Momentum.X *= coeff;

			if ((_Direction & FrictionDirection.Vertical) != 0)
				obj.Momentum.Y *= coeff;
		}

		#endregion
	}

	/// <summary>
	/// The friction direction.
	/// </summary>
	[Flags]
	public enum FrictionDirection
	{
		Horizontal	= 0x01,
		Vertical	= 0x02,

		Both		= Horizontal | Vertical
	}
}
