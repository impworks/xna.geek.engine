using geek.GameEngine.Visuals;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The basic class that allows animating arbitrary properties.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class PropertyAnimator<T>: IPropertyAnimator
	{
		#region Constructors

		protected PropertyAnimator(T initial, T desired, float time, AnimatableProperty pty, InterpolationMode mode)
		{
			_InitialValue = initial;
			_DesiredValue = desired;
			_DesiredTime = time;
			_Mode = mode;
			_Method = Interpolate.GetMethod(mode);

			Property = pty;
		}

		#endregion

		#region Fields

        /// <summary>
        /// A flag indicating the current animation is suspended until the flag is reset.
        /// </summary>
	    public bool Paused { get; set; }

		/// <summary>
		/// Gets the flag that indicates the effect is finished.
		/// </summary>
		public bool Finished { get { return _ElapsedTime >= _DesiredTime; } }

		/// <summary>
		/// Gets the property that should be animated.
		/// </summary>
		public AnimatableProperty Property { get; protected set; }

		/// <summary>
		/// The value to animate from.
		/// </summary>
		protected T _InitialValue;

		/// <summary>
		/// The value to animate to.
		/// </summary>
		protected T _DesiredValue;

		/// <summary>
		/// The desired length of the effect.
		/// </summary>
		protected float _DesiredTime;

		/// <summary>
		/// Time that has currently elapsed.
		/// </summary>
		protected float _ElapsedTime;

		/// <summary>
		/// The interpolation method.
		/// </summary>
		protected InterpolationMethod _Method;

		/// <summary>
		/// The interpolation mode corresponding to the method.
		/// </summary>
		protected InterpolationMode _Mode;

		/// <summary>
		/// Animate the object back to it's original state after the animation has finished.
		/// </summary>
		public bool AnimateBack;

		#endregion

		#region IPropertyAnimator implementation

		public void UpdateProperty(DynamicObject obj)
		{
			if (Finished)
				return;

			_ElapsedTime += GameCore.Delta;
			var curr = interpolateProperty();
			setProperty(obj, curr);

			if (Finished && AnimateBack)
				GameCore.RegisterDeferredAction(() => revertAnimation(obj));
		}

		public void SkipAnimation(DynamicObject obj)
		{
			if (Finished) return;

			_ElapsedTime = _DesiredTime;
			setProperty(obj, AnimateBack ? _InitialValue : _DesiredValue);
		}

		#endregion

		#region Methods

		/// <summary>
		/// The method that is overridden in child classes for converting T into a float or list of floats.
		/// </summary>
		/// <returns></returns>
		protected abstract T interpolateProperty();

		/// <summary>
		/// This method is overridden in child classes for setting the interpolated property to it's place.
		/// </summary>
		/// <param name="obj">Object that contains the property.</param>
		/// <param name="value">New value.</param>
		protected abstract void setProperty(DynamicObject obj, T value);

		/// <summary>
		/// This method is overridden in child classes for animating the property back if the "animateBack" flag was set.
		/// </summary>
		protected abstract void revertAnimation(DynamicObject obj);

		/// <summary>
		/// Interpolate a float number using a tweening method.
		/// </summary>
		/// <param name="initialConverted">Float representation of the initial value.</param>
		/// <param name="desiredConverted">Float representation of the desired value.</param>
		/// <returns></returns>
		protected float interpolateFloat(float initialConverted, float desiredConverted)
		{
			return _Method(initialConverted, desiredConverted, _ElapsedTime/_DesiredTime);
		}

		#endregion
	}
}
