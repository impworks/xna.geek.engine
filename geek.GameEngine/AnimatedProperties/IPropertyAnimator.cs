using geek.GameEngine.Visuals;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The base interface for all property animators.
	/// </summary>
	public interface IPropertyAnimator
	{
		/// <summary>
		/// Checks whether the animation is suspended.
		/// </summary>
		bool Paused { get; set; }

		/// <summary>
		/// Checks whether the effect has finished.
		/// </summary>
		/// <returns></returns>
		bool Finished { get; }

		/// <summary>
		/// Gets the property that is being animated.
		/// </summary>
		AnimatableProperty Property { get; }

		/// <summary>
		/// Update the the property.
		/// The property animator stores the property, easing, time and value ranges internally.
		/// </summary>
		/// <param name="obj">Object to update.</param>
		void UpdateProperty(DynamicObject obj);

		/// <summary>
		/// Finish the animation and jump straight to it's final value.
		/// </summary>
		void SkipAnimation(DynamicObject obj);
	}
}
