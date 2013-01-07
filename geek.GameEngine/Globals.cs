using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine
{
	/// <summary>
	/// Contains various constants and globally used miscelanities.
	/// </summary>
	public static class Globals
	{
		static Globals()
		{
			_Random = new Random();	
		}

		#region Float comparison
		
		/// <summary>
		/// The mininum value to take into account when modifying object properties.
		/// Prevents coordinates, angle, etc from jitter degradation over time.
		/// </summary>
		public const float Epsilon = 0.0001f;

		/// <summary>
		/// Check whether a number is too small to account for.
		/// </summary>
		/// <param name="number">Number.</param>
		/// <returns></returns>
		public static bool IsAlmostNull(this float number)
		{
			return Math.Abs(number) < Epsilon;
		}

		/// <summary>
		/// Check whether two floating point numbers are almost equal (to a given precision).
		/// </summary>
		/// <param name="number">Number</param>
		/// <param name="compareTo">Other number</param>
		/// <param name="precision">Precision.</param>
		/// <returns></returns>
		public static bool IsAlmost(this float number, float compareTo, float precision = Epsilon)
		{
			return (number <= compareTo + precision) && (number >= compareTo - precision);
		}

		#endregion

		#region Vectors and directions

		/// <summary>
		/// Create a vector based on length and direction.
		/// </summary>
		/// <param name="length">Desired vector length.</param>
		/// <param name="direction">Desired vector angle in radians.</param>
		public static Vector2 CreateVector(float length, float direction)
		{
			return new Vector2(length * (float)Math.Cos(direction), length * (float)Math.Sin(direction));
		}

		/// <summary>
		/// Get a direction from one point to another.
		/// </summary>
		public static float DirectionTo(Vector2 point1, Vector2 point2)
		{
			var vec = point2 - point1;
			return (float)Math.Atan2(vec.Y, vec.X);
		}

		/// <summary>
		/// Get the direction from one object to another.
		/// </summary>
		public static float DirectionTo(VisualObjectBase obj1, VisualObjectBase obj2)
		{
			return DirectionTo(obj1.AbsolutePosition, obj2.AbsolutePosition);
		}

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		public static float Distance(Vector2 point1, Vector2 point2)
		{
			var x = point1.X - point2.X;
			var y = point1.Y - point2.Y;
			return (float)Math.Sqrt(x*x + y*y);
		}

		#endregion

		#region Randomization

		/// <summary>
		/// Random generator.
		/// </summary>
		private static readonly Random _Random;

		/// <summary>
		/// Creates a random number between 0.0 and 1.0.
		/// </summary>
		/// <returns></returns>
		public static float Random()
		{
			return (float)_Random.NextDouble();
		}

		/// <summary>
		/// Creates a random number between two values.
		/// </summary>
		/// <param name="from">Lower bound.</param>
		/// <param name="to">Upped bound.</param>
		/// <returns></returns>
		public static float Random(float from, float to)
		{
			var scale = to - from;
			return from + (float)_Random.NextDouble()*scale;
		}

		/// <summary>
		/// Creates a random integer in the given range.
		/// </summary>
		/// <param name="from">Lower bound.</param>
		/// <param name="to">Upped bound.</param>
		/// <returns></returns>
		public static int RandomInt(int from, int to)
		{
			return _Random.Next(from, to);
		}

		/// <summary>
		/// Flips a virtual coin.
		/// </summary>
		public static bool RandomBool()
		{
			return _Random.NextDouble() > 0.5;
		}

		/// <summary>
		/// Random sign: plus or minus.
		/// </summary>
		/// <returns></returns>
		public static int RandomSign()
		{
			return RandomBool() ? 1 : -1;
		}

		/// <summary>
		/// Pick a random item from the array.
		/// </summary>
		/// <typeparam name="T">Data type.</typeparam>
		/// <param name="data">Data.</param>
		/// <returns></returns>
		public static T RandomPick<T>(params T[] data)
		{
			if (data.Length == 0)
				return default(T);

			// works better on bigger numbers
			var id = _Random.Next(data.Length*100);
			return data[id/100];
		}

		/// <summary>
		/// Pick a random item from the list.
		/// </summary>
		/// <typeparam name="T">Data type.</typeparam>
		/// <param name="data">Data.</param>
		/// <returns></returns>
		public static T RandomPick<T>(List<T> data)
		{
			if (data.Count == 0)
				return default(T);

			// works better on bigger numbers
			var id = _Random.Next(data.Count * 100);
			return data[id / 100];
		}

		/// <summary>
		/// Shuffles the array or a list.
		/// </summary>
		public static void Shuffle<T>(IList<T> source)
		{
			for (var i = 0; i < source.Count; i++)
			{
				var j = _Random.Next(i);
				var tmp = source[i];
				source[i] = source[j];
				source[j] = tmp;
			}
		}

		#endregion
	}
}
