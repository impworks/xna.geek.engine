using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The object wobbles around a spot.
	/// </summary>
	public class JitterBehaviour : IBehaviour
	{
		#region Constructor

		public JitterBehaviour(int jitterAmount = 1, float rate = 50)
		{
			JitterAmount = jitterAmount;
			JitterRate = rate;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The rate of jitter.
		/// </summary>
		public float JitterRate;

		/// <summary>
		/// The amount of jitter to apply to object's position.
		/// </summary>
		public int JitterAmount;

		/// <summary>
		/// The position around which the object should jitter.
		/// </summary>
		private Vector2? _OriginalPosition;

		/// <summary>
		/// Time elapsed since last jitter.
		/// </summary>
		private float _ElapsedTime;

		#endregion

		#region Methods
		
		public void UpdateObjectState(DynamicObject obj)
		{
			if (_OriginalPosition == null)
				_OriginalPosition = obj.Position;

			_ElapsedTime += GameCore.Delta;
			var time = 1/JitterRate;
			if (_ElapsedTime > time)
			{
				_ElapsedTime -= time;
				obj.Position = new Vector2(
					_OriginalPosition.Value.X + Globals.RandomInt(-JitterAmount, JitterAmount),
					_OriginalPosition.Value.Y + Globals.RandomInt(-JitterAmount, JitterAmount)
				);
			}
		}

		#endregion
	}
}
