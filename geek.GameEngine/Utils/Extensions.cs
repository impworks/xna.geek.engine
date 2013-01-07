using System;
using System.Globalization;
using System.Collections.Generic;

namespace geek.GameEngine.Utils
{
	/// <summary>
	/// Common extensions.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Execute an action in a list of values (fluent style).
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> list, Action<T> act)
		{
			foreach (var curr in list)
				act(curr);
		}

		/// <summary>
		/// Execute an action on non-null items from a list.
		/// </summary>
		public static void ForEachNotNull<T>(this IEnumerable<T> list, Action<T> act) where T: class
		{
			foreach (var curr in list)
				if (curr != null)
					act(curr);
		}

		/// <summary>
		/// Checks if an enum has a flag.
		/// </summary>
		public static bool HasFlag<T>(this T value, T flag) where T: struct, IConvertible
		{
			if(!typeof(T).IsEnum)
				throw new ArgumentException("Not an enum!");

			return (value.ToInt32(CultureInfo.InvariantCulture) & flag.ToInt32(CultureInfo.InvariantCulture)) != 0;
		}

		/// <summary>
		/// Gets a value with jitter.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="coeff">Jitter percentage.</param>
		public static float Jitter(this float value, float coeff = 0.1f)
		{
			var jitter = value*coeff;
			return value + Globals.Random(-jitter, jitter);
		}

		/// <summary>
		/// Gets an integer value with jitter.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="range">Hitter amount.</param>
		/// <returns></returns>
		public static int Jitter(this int value, int range)
		{
			return value + Globals.RandomInt(-range, range);
		}
	}
}
