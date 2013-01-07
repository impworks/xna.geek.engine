using System;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The class that allows animating DynamicObject's float properties.
	/// </summary>
	public class CustomPropertyAnimator : PropertyAnimator<float>
	{
		#region Constructors

		public CustomPropertyAnimator(float initial,
									  Action<DynamicObject, float> setter,
									  float desired,
									  float time,
									  InterpolationMode mode = InterpolationMode.Linear)
			: base(initial, desired, time, AnimatableProperty.Custom, mode)
		{
			_Setter = setter;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The setter that modifies the object's property.
		/// </summary>
		private readonly Action<DynamicObject, float> _Setter;

		#endregion

		#region PropertyAnimator overrides

		protected override float interpolateProperty()
		{
			return interpolateFloat(_InitialValue, _DesiredValue);
		}

		protected override void setProperty(DynamicObject obj, float value)
		{
			_Setter(obj, value);
		}

		protected override void revertAnimation(DynamicObject obj)
		{
			obj.AnimateProperty(_Setter, _DesiredValue, _InitialValue, _DesiredTime, _Mode);
		}

		#endregion
	}
}
