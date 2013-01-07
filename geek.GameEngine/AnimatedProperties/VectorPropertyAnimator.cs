using System;
using geek.GameEngine.Visuals;
using Microsoft.Xna.Framework;

namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The class that allows animating DynamicObject's vector properties.
	/// </summary>
	public class VectorPropertyAnimator : PropertyAnimator<Vector2>
	{
		#region Constructors

		public VectorPropertyAnimator(DynamicObject obj, Vector2 desired, float time, AnimatableProperty pty, InterpolationMode mode)
			: base(getProperty(obj, pty), desired, time, pty, mode)
		{ }

		#endregion

		#region PropertyAnimator overrides

		protected override Vector2 interpolateProperty()
		{
			return new Vector2(
				interpolateFloat(_InitialValue.X, _DesiredValue.X),
				interpolateFloat(_InitialValue.Y, _DesiredValue.Y)
			);
		}

		protected override void setProperty(DynamicObject obj, Vector2 value)
		{
			if (Property == AnimatableProperty.Position)
			{
				obj.Momentum = (value - obj.Position)/GameCore.Delta;

				// Stop the object after it has moved to destination point.
				if (Finished)
					GameCore.RegisterDeferredAction(() => obj.Momentum = Vector2.Zero);
			}
			else if (Property == AnimatableProperty.ScaleVector)
			{
				obj.ScaleVector = value;
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
		private static Vector2 getProperty(DynamicObject obj, AnimatableProperty property)
		{
			if(property == AnimatableProperty.Position)
				return obj.Position;
			if (property == AnimatableProperty.ScaleVector)
				return obj.ScaleVector ?? new Vector2(obj.Scale);

			throw new InvalidOperationException(string.Format("The {0} property is not of vector type!", property));
		}

		#endregion
	}
}