using Microsoft.Xna.Framework;

namespace geek.GameEngine.Utils
{
	/// <summary>
	/// A structure that represents a value and jitter around it.
	/// </summary>
	public struct JitteryVector2
	{
		#region Constructor

		/// <summary>
		/// Create a new jittery vector instance.
		/// </summary>
		/// <param name="medianX">X median value.</param>
		/// <param name="medianY">Y median value.</param>
		/// <param name="jitter">Jitter value.</param>
		public JitteryVector2(float medianX, float medianY, float jitter = 0)
		{
			Median = new Vector2(medianX, medianY);
			Jitter = jitter;
		}

		/// <summary>
		/// Create a new jittery vector instance.
		/// </summary>
		/// <param name="median">Median value.</param>
		/// <param name="jitter">Jitter value.</param>
		public JitteryVector2(Vector2 median, float jitter = 0)
		{
			Median = median;
			Jitter = jitter;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Median of the vector.
		/// </summary>
		public Vector2 Median;

		/// <summary>
		/// Spread coefficient.
		/// </summary>
		public float Jitter;

		#endregion

		#region Methods

		/// <summary>
		/// Get a new vector that falls into allowed jittery range.
		/// </summary>
		/// <returns></returns>
		public Vector2 GetValue()
		{
			if (Jitter.IsAlmostNull())
				return Median;

			var jX = Globals.Random(-Jitter, Jitter);
			var jY = Globals.Random(-Jitter, Jitter);
			return new Vector2(Median.X + jX, Median.Y + jY);
		}

		/// <summary>
		/// Assign the value just as a float if no jitter is needed.
		/// </summary>
		/// <param name="value">Median value.</param>
		/// <returns></returns>
		public static implicit operator JitteryVector2(Vector2 value)
		{
			return new JitteryVector2(value);
		}

		#endregion
	}
}