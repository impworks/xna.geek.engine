using geek.GameEngine.AnimatedProperties;

namespace geek.GameEngine.Utils
{
	/// <summary>
	/// A tween state control for storing the state of an interpolation.
	/// </summary>
	public class Tweener
	{
		#region Constructor

		public Tweener(float from, float to, float time, InterpolationMode mode = InterpolationMode.Linear)
		{
			_From = from;
			_To = to;
			_DesiredTime = time;
			_Method = Interpolate.GetMethod(mode);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Starting value.
		/// </summary>
		private readonly float _From;

		/// <summary>
		/// Desired value.
		/// </summary>
		private readonly float _To;

		/// <summary>
		/// Time to span the interpolation across.
		/// </summary>
		private readonly float _DesiredTime;

		/// <summary>
		/// Currently elapsed time.
		/// </summary>
		private float _ElapsedTime;

		/// <summary>
		/// Interpolation method.
		/// </summary>
		private readonly InterpolationMethod _Method;

		/// <summary>
		/// Gets the current value.
		/// </summary>
		public float Value
		{
			get { return _Method(_From, _To, _ElapsedTime / _DesiredTime); }
		}

		/// <summary>
		/// Gets the finished flag.
		/// </summary>
		public bool IsFinished
		{
			get { return _ElapsedTime >= _DesiredTime; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Update the tween state.
		/// </summary>
		/// <param name="delta">Game time delta.</param>
		public void Update(float delta)
		{
			if(!IsFinished)
				_ElapsedTime += delta;
		}

		#endregion
	}
}
