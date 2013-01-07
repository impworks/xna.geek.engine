namespace geek.GameEngine.Utils
{
	/// <summary>
	/// A structure that represents a value and jitter around it.
	/// </summary>
	public struct JitteryValue
	{
		#region Constructor

		/// <summary>
		/// Create a new jittery value instance.
		/// </summary>
		/// <param name="median">Median value.</param>
		/// <param name="jitter">Jitter value.</param>
		public JitteryValue(float median, float jitter = 0)
		{
			Median = median;
			Jitter = jitter;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Center of the value spread.
		/// </summary>
		public float Median;

		/// <summary>
		/// Spread coefficient.
		/// </summary>
		public float Jitter;

		#endregion

		#region Methods

		/// <summary>
		/// Get a new value that falls into allowed jittery range.
		/// </summary>
		/// <returns></returns>
		public float GetValue()
		{
			if (Jitter.IsAlmostNull())
				return Median;

			return Median + Globals.Random(-Jitter, Jitter);
		}

		/// <summary>
		/// Assign the value just as a float if no jitter is needed.
		/// </summary>
		/// <param name="value">Median value.</param>
		/// <returns></returns>
		public static implicit operator JitteryValue(float value)
		{
			return new JitteryValue(value);
		}

		/// <summary>
		/// Assign the value just as a float if no jitter is needed.
		/// </summary>
		/// <param name="value">Median value.</param>
		/// <returns></returns>
		public static implicit operator JitteryValue(int value)
		{
			return new JitteryValue(value);
		}

		#endregion
	}
}