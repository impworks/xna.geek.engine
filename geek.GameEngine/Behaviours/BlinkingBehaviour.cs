using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The effect that makes the object blink N times per given time.
	/// </summary>
	public class BlinkingBehaviour : IBehaviour
	{
		#region Constructor

		/// <summary>
		/// Create a new instance of blinking behaviour.
		/// </summary>
		/// <param name="times">Number of times to blink.</param>
		/// <param name="duration">Blinking duration.</param>
		public BlinkingBehaviour(int times, float duration)
		{
			_FlipsLeft = times > 0 ? times*2 : -1;
			_FlipTime = duration/_FlipsLeft;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Time elapsed since last 'flip' of TransparencyShift.
		/// </summary>
		private float _ElapsedTime;

		/// <summary>
		/// Time to elapse when a new 'flip' is required.
		/// </summary>
		private readonly float _FlipTime;

		/// <summary>
		/// Number of flips left.
		/// </summary>
		private int _FlipsLeft;

		#endregion

		#region Methods

		public void UpdateObjectState(DynamicObject obj)
		{
			_ElapsedTime += GameCore.Delta;
			if (_ElapsedTime >= _FlipTime)
			{
				obj.TransparencyShift = (_FlipsLeft & 1) == 0 ? -1/_FlipTime : 1/_FlipTime;
				_ElapsedTime = 0;

				if(_FlipsLeft > 0)
					_FlipsLeft--;
			}

			if (_FlipsLeft == 0)
				GameCore.RegisterDeferredAction(() => obj.Behaviours.Remove<BlinkingBehaviour>());
		}

		#endregion
	}
}
