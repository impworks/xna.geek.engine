using System;
using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The class that allows animating DynamicObject's TintColor property.
	/// </summary>
	public class ColorPropertyAnimator : PropertyAnimator<Color>
	{
		public ColorPropertyAnimator(DynamicObject obj, Color desired, float time, AnimatableProperty pty, InterpolationMode mode)
			: base(obj.TintColor, desired, time, pty, mode)
		{
			
		}

		#region PropertyAnimator overrides

		protected override Color interpolateProperty()
		{
			return new Color(
					(byte)interpolateFloat(_InitialValue.R, _DesiredValue.R),
					(byte)interpolateFloat(_InitialValue.G, _DesiredValue.G),
					(byte)interpolateFloat(_InitialValue.B, _DesiredValue.B),
					(byte)interpolateFloat(_InitialValue.A, _DesiredValue.A)
				);
		}

		protected override void setProperty(DynamicObject obj, Color value)
		{
			if(Property != AnimatableProperty.Color)
				throw new InvalidOperationException(string.Format("The {0} property is not of color type!", Property));

			obj.TintColor = value;
		}

		protected override void revertAnimation(DynamicObject obj)
		{
			obj.AnimateProperty(_InitialValue, _DesiredTime, _Mode);
		}

		#endregion
	}
}
