using System;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The class that allows animating DynamicObject's float properties.
	/// </summary>
	public class FloatPropertyAnimator : PropertyAnimator<float>
	{
		#region Constructors
		
		public FloatPropertyAnimator(DynamicObject obj, float desired, float time, AnimatableProperty pty, InterpolationMode mode)
			: base(getProperty(obj, pty), desired, time, pty, mode)
		{ }

		#endregion

		#region PropertyAnimator overrides

		protected override float interpolateProperty()
		{
			return interpolateFloat(_InitialValue, _DesiredValue);
		}

		protected override void setProperty(DynamicObject obj, float value)
		{
			switch(Property)
			{
				case AnimatableProperty.Angle:			obj.Angle = value; break;
				case AnimatableProperty.Scale:			obj.Scale = value; break;
				case AnimatableProperty.Transparency:	obj.Transparency = value; break;
				case AnimatableProperty.Speed:			obj.Speed = value; break;
				case AnimatableProperty.Direction:		obj.Direction = value; break;

				default: throw new InvalidOperationException(string.Format("The {0} property is not of float type!", Property));
			}
		}

		protected override void revertAnimation(DynamicObject obj)
		{
			obj.AnimateProperty(Property, _InitialValue, _DesiredTime, _Mode);
		}

		/// <summary>
		/// Get a property from the object.
		/// </summary>
		/// <param name="obj">Object to get the property from.</param>
		/// <param name="property">Property to get.</param>
		/// <returns></returns>
		private static float getProperty(DynamicObject obj, AnimatableProperty property)
		{
			switch (property)
			{
				case AnimatableProperty.Angle:			return obj.Angle;
				case AnimatableProperty.Scale:			return obj.Scale;
				case AnimatableProperty.Transparency:	return obj.Transparency;
				case AnimatableProperty.Direction:		return obj.Direction;
				case AnimatableProperty.Speed:			return obj.Speed;

				default: throw new InvalidOperationException(string.Format("The {0} property is not of float type!", property));
			}
		}

		#endregion
	}
}
