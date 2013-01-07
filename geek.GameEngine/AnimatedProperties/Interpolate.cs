using System;
using Microsoft.Xna.Framework;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The class that provides common interpolation patterns for a value in a range.
	/// </summary>
	public static class Interpolate
	{
		#region Linear interpolation

		//    Linear interpolation graph:
		//
		//    max
		//     |       /
		//     |      /
		//     |     /
		//     |    /
		//     |   /
		//     |  /
		//     | /
		//   0 |/________ time
		//    min

		/// <summary>
		/// Linearly interpolate a value between two points:
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float Linear(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min + distance * tweenState;
		}

		#endregion

		#region Power-based interpolation curve

		//    Power-based ease in graph:
		//
		//    max
		//     |        
		//     |          |
		//     |          |
		//     |         |
		//     |        /
		//     |      ,'
		//     |___--'
		//   0 |_________ time
		//    min

		/// <summary>
		/// Smoothly interpolate a value with square easing in.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseInSoft(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min + distance * (tweenState * tweenState);
		}

		/// <summary>
		/// Smoothly interpolate a value with cubic easing in.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseInMedium(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min + distance * (tweenState * tweenState * tweenState);
		}

		/// <summary>
		/// Smoothly interpolate a value with quartic easing in.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseInHard(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min + distance * (tweenState * tweenState * tweenState * tweenState);
		}

		//    Power-based ease out graph:
		//
		//    max
		//     |        _---
		//     |      ,'
		//     |    ,'
		//     |   /
		//     |  /
		//     | |
		//     ||
		//   0 ||_________ time
		//    min

		/// <summary>
		/// Smoothly interpolate a value with quadratic easing out.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseOutSoft(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min - distance*tweenState*(tweenState - 2);
		}

		/// <summary>
		/// Smoothly interpolate a value with cubic easing out.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseOutMedium(float min, float max, float tweenState)
		{
			var distance = max - min;
			var t = tweenState - 1;
			return min + distance*(t*t*t + 1);
		}

		/// <summary>
		/// Smoothly interpolate a value with quartic easing out.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseOutHard(float min, float max, float tweenState)
		{
			var distance = max - min;
			tweenState--;
			return min - distance * (tweenState * tweenState * tweenState * tweenState - 1);
		}

		//    Power-based ease in & out graph:
		//
		//    max
		//     |         ,-
		//     |       ,'
		//     |      /
		//     |     |
		//     |     |
		//     |    /
		//     |  ,'
		//   0 |-'________ time
		//    min

		/// <summary>
		/// Smoothly interpolate a value with quadratic easing on both points.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseBothSoft(float min, float max, float tweenState)
		{
			tweenState *= 2;
			var distance = max - min;
			if (tweenState < 1)
				return min + distance * 0.5f * tweenState * tweenState;

			tweenState--;
			return min - distance * 0.5f * (tweenState * (tweenState - 2) - 1);
		}

		/// <summary>
		/// Smoothly interpolate a value with cubic easing on both points.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseBothMedium(float min, float max, float tweenState)
		{
			tweenState *= 2;
			var distance = max - min;
			if (tweenState < 1)
				return min + distance * 0.5f * tweenState * tweenState * tweenState;

			tweenState -= 2;
			return min + distance * 0.5f * (tweenState * tweenState * tweenState + 2);
		}

		/// <summary>
		/// Smoothly interpolate a value with quartic easing on both points.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float EaseBothHard(float min, float max, float tweenState)
		{
			tweenState *= 2;
			var distance = max - min;
			if (tweenState < 1)
				return min + distance * 0.5f * tweenState * tweenState * tweenState * tweenState;

			tweenState -= 2;
			return min - distance * 0.5f * (tweenState * tweenState * tweenState * tweenState - 2);
		}

		#endregion

		#region Back-retracking interpolation

		//    Back-retracking graph (both):
		//    
		//      |        ..
		//  max |       /  `
		//      |      |
		//      |     |
		//      |    |
		//  min |.  /
		//      | '' 
		//   0  |__________ time
		//    

		private const float Back_S1 = 1.70158f;
		private const float Back_S2 = 2.59491f;

		/// <summary>
		/// Smoothly interpolate a value, retracking back a bit on start.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float BackIn(float min, float max, float tweenState)
		{
			var distance = max - min;
			return min + distance*tweenState*tweenState*((Back_S1 + 1)*tweenState - Back_S1);
		}

		/// <summary>
		/// Smoothly interpolate a value, overshooting and retracking back a bit on end.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float BackOut(float min, float max, float tweenState)
		{
			var distance = max - min;
			tweenState--;
			return min + distance * (tweenState * tweenState * ((Back_S1 + 1) * tweenState + Back_S1) + 1);
		}

		/// <summary>
		/// Smoothly interpolate a value, retracking back a bit on both start and end.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float BackBoth(float min, float max, float tweenState)
		{
			var distance = max - min;
			tweenState *= 2;

			if (tweenState < 1)
				return min + distance * 0.5f * (tweenState * tweenState * ((Back_S2 + 1) * tweenState - Back_S2));

			tweenState -= 2;
			return min + distance * 0.5f * (tweenState * tweenState * ((Back_S2 + 1) * tweenState + Back_S2) + 2);
		}

		#endregion

		#region Elastic interpolation

		//    Elastic interpolation graph (ease-out):
		//    
		//      |  ,'`.
		//      | /    \  .^--
		//  max | |     `'
		//      | |
		//      | |
		//      ||
		//      ||
		//      || 
		//   0  ||__________ time
		//    min

		/// <summary>
		/// Smoothly interpolate the object, having a few ripples at the end.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float Elastic(float min, float max, float tweenState)
		{
			if (tweenState.IsAlmostNull())
				return min;
			if (tweenState.IsAlmost(1))
				return max;

			var distance = max - min;
			return max + (float)(distance * Math.Pow(2, -10 * tweenState) * Math.Sin(4.666f * MathHelper.Pi * tweenState - MathHelper.PiOver2));
		}

		#endregion

		#region Bounce interpolation

		//    Bounce interpolation graph (ease-out):
		//    
		//      |                   
		//  max |      |\      /\   /\_/
		//      |      | \    /  `-'
		//      |      |  `--'
		//      |      |
		//      |     |
		//      |    /
		//      |   /
		//   0  |--'___________________ time
		//    min

		/// <summary>
		/// Smoothly interpolate the value, bouncing a few times at the end.
		/// </summary>
		/// <param name="min">Range start.</param>
		/// <param name="max">Range end.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float Bounce(float min, float max, float tweenState)
		{
			const float span = 2.75f;
			const float weight = 7.5625f;

			var distance = max - min;

			if (tweenState < 1.0f / span)
				return min + distance*tweenState*tweenState*weight;

			if(tweenState < 2.0f / span)
			{
				tweenState -= 1.5f / span;
				return min + distance*(tweenState*tweenState*weight + 0.75f);
			}

			if(tweenState < 2.5f / span)
			{
				tweenState -= 2.25f / span;
				return min + distance*(tweenState*tweenState*weight + 0.9375f);
			}

			tweenState -= 2.625f / span;
			return min + distance*(tweenState*tweenState*weight + 0.984375f);
		}

		#endregion

		#region Sine wave pulse interpolation

		//          Sine wave pulse graph:
		//
		//          |
		//          |
		//  +amount |  ,--.
		//          | /    \
		//  state   |/      \      
		//          |        \    /
		//  -amount |         `--'
		//          |
		//        0 |_______________ time
		//          0

		/// <summary>
		/// Pulse around a value using sine waveform.
		/// </summary>
		/// <param name="state">Value to sine-pulse around.</param>
		/// <param name="amount">Sine-pulse power.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float SinePulse(float state, float amount, float tweenState)
		{
			var sinePoint = MathHelper.TwoPi * tweenState;
			return state + (float) Math.Sin(sinePoint)*amount;
		}

		/// <summary>
		/// Pulse around a value using inverse sine waveform.
		/// </summary>
		/// <param name="state">Value to sine-pulse around.</param>
		/// <param name="amount">Sine-pulse power.</param>
		/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
		/// <returns></returns>
		public static float SinePulseBack(float state, float amount, float tweenState)
		{
			var sinePoint = MathHelper.TwoPi * tweenState;
			return state - (float)Math.Sin(sinePoint) * amount;
		}

		#endregion

		/// <summary>
		/// Get appropriate method for an interpolation mode.
		/// </summary>
		/// <param name="mode">Interpolation mode</param>
		/// <returns></returns>
		public static InterpolationMethod GetMethod(InterpolationMode mode)
		{
			switch (mode)
			{
				case InterpolationMode.EaseInSoft:			return EaseInSoft;
				case InterpolationMode.EaseOutSoft:			return EaseOutSoft;
				case InterpolationMode.EaseBothSoft:		return EaseBothSoft;
				case InterpolationMode.EaseInMedium:		return EaseInMedium;
				case InterpolationMode.EaseOutMedium:		return EaseOutMedium;
				case InterpolationMode.EaseBothMedium:		return EaseBothMedium;
				case InterpolationMode.EaseInHard:			return EaseInSoft;
				case InterpolationMode.EaseOutHard:			return EaseOutHard;
				case InterpolationMode.EaseBothHard:		return EaseBothHard;
				case InterpolationMode.BackIn:				return BackIn;
				case InterpolationMode.BackOut:				return BackOut;
				case InterpolationMode.BackBoth:			return BackBoth;
				case InterpolationMode.Bounce:				return Bounce;
				case InterpolationMode.Elastic:				return Elastic;
				case InterpolationMode.SinePulse:			return SinePulse;
				case InterpolationMode.SinePulseBack:		return SinePulseBack;

				default:									return Linear;
			}
		}
	}

	/// <summary>
	/// The interface for all tweening methods.
	/// </summary>
	/// <param name="min">Value range start.</param>
	/// <param name="max">Value range end.</param>
	/// <param name="tweenState">Tweening state between 0 (not applied) and 1 (completed).</param>
	/// <returns></returns>
	public delegate float InterpolationMethod(float min, float max, float tweenState);
}
