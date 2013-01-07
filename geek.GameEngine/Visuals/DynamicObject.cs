using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.AnimatedProperties;
using geek.GameEngine.Behaviours;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// A class representing an object that can move, rotate, scale and change transparency.
	/// </summary>
	public abstract class DynamicObject : VisualObjectBase
	{
		#region Constructors

		protected DynamicObject()
		{
			Behaviours = new BehaviourManager();
			_AnimatedProperties = new List<IPropertyAnimator>();
			BlendState = BlendState.AlphaBlend;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Sprite scale. 1 = normal size.
		/// </summary>
		public float Scale = 1.0f;

		/// <summary>
		/// Scale derivative:
		/// Scale is incremented or decremented by this value each second.
		/// </summary>
		public float ScaleShift;

		/// <summary>
		/// Sprite scale that can be different for X and Y axis.
		/// </summary>
		public Vector2? ScaleVector;

		/// <summary>
		/// Sprite angle in radians.
		/// </summary>
		public float Angle;

		/// <summary>
		/// Angle derivative.
		/// Sprite is rotated by this value each second.
		/// </summary>
		public float AngleShift;

		/// <summary>
		/// Sprite momentum.
		/// Sprite is moved by this value each second.
		/// </summary>
		public Vector2 Momentum;

		/// <summary>
		/// Sprite tint color.
		/// Default is White (No tint).
		/// </summary>
		public Color TintColor = Color.White;

		/// <summary>
		/// The behaviour manager for current object.
		/// </summary>
		public BehaviourManager Behaviours { get; private set; }

		/// <summary>
		/// The list of property animators that are applied to the object on each pass.
		/// </summary>
		private readonly List<IPropertyAnimator> _AnimatedProperties;

		/// <summary>
		/// Blending mode.
		/// </summary>
		public BlendState BlendState;

		/// <summary>
		/// Sprite transparency: 0 = fully transparent, 1 = fully opaque.
		/// Default is 1.
		/// </summary>
		public float Transparency
		{
			get { return TintColor.A/255.0f; }
			set
			{
				var b = (byte) (MathHelper.Clamp(value, 0, 1)*255);
				TintColor.A = TintColor.R = TintColor.G = TintColor.B = b;
			}
		}

		/// <summary>
		/// Transparency derivative.
		/// The transparency is altered by this value each second.
		/// </summary>
		public float TransparencyShift;

		/// <summary>
		/// Gets or sets object's speed.
		/// </summary>
		public float Speed
		{
			get { return Momentum.Length(); }
			set
			{
				if (Momentum.X.IsAlmostNull() && Momentum.Y.IsAlmostNull())
				{
					Momentum = new Vector2(value, 0);
				} 
				else
				{
					Momentum.Normalize();
					Momentum *= value;
				}
			}
		}

		/// <summary>
		/// Speed derivative.
		/// </summary>
		public float Acceleration;

		/// <summary>
		/// Gets or sets the object's direction.
		/// </summary>
		public float Direction
		{
			get { return (float)Math.Atan2(Momentum.X, Momentum.Y) - MathHelper.PiOver2; }
			set { Momentum = Globals.CreateVector(Momentum.Length(), value ); }
		}

		/// <summary>
		/// Direction derivative.
		/// </summary>
		public float AngularAcceleration;

		/// <summary>
		/// 
		/// </summary>
		public PauseTarget Pause;

		/// <summary>
		/// Has the object been disposed already?
		/// </summary>
		public bool IsFadingOut { get; private set; }

		#endregion

		#region VisualObjectBase overrides

		/// <summary>
		/// Applies value derivatives to values.
		/// </summary>
		public override void Update()
		{
			if((Pause & PauseTarget.Behaviours) == 0)
				Behaviours.UpdateObjectState(this);

			if ((Pause & PauseTarget.PropertyAnimation) == 0)
				updateAnimatedProperties();

			var delta = GameCore.Delta;

			if ((Pause & PauseTarget.Transformation) == 0)
			{
				if (!ScaleShift.IsAlmostNull())
					Scale += delta*ScaleShift;

				if (!AngleShift.IsAlmostNull())
					Angle += delta*AngleShift;

				if (!TransparencyShift.IsAlmostNull())
					Transparency += delta*TransparencyShift;

				if (!Acceleration.IsAlmostNull())
					Speed += delta*Acceleration;

				if (!AngularAcceleration.IsAlmostNull())
					Direction += delta*AngularAcceleration;
			}

			if ((Pause & PauseTarget.Momentum) == 0)
				if (!Momentum.X.IsAlmostNull() || !Momentum.Y.IsAlmostNull())
					Position += delta * Momentum;
		}

		public override void Draw(SpriteBatch batch)
		{
			GameCore.UpdateBlendingState(BlendState);
		}

		#endregion

		#region Motion methods

		/// <summary>
		/// Stop the object.
		/// </summary>
		public void Stop()
		{
			Momentum = Vector2.Zero;
			StopAnimatingProperty(AnimatableProperty.Position, false);
		}

		/// <summary>
		/// Make the object move in a given direction at a given speed.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="angle">Direction.</param>
		public void Move(float speed, float angle)
		{
			Momentum = Globals.CreateVector(speed, angle);
		}

		/// <summary>
		/// Move the object towards another object.
		/// </summary>
		/// <param name="obj">Object to move towards.</param>
		public void MoveTowards(VisualObjectBase obj)
		{
			Direction = Globals.DirectionTo(AbsolutePosition, obj.AbsolutePosition);
		}

		/// <summary>
		/// Move the object towards a point.
		/// </summary>
		/// <param name="point">Desired point.</param>
		public void MoveTowards(Vector2 point)
		{
			Direction = Globals.DirectionTo(AbsolutePosition, point);
		}

		/// <summary>
		/// Move to a specified point in a given amount of time.
		/// </summary>
		/// <param name="point">Destination point.</param>
		/// <param name="time">Movement duration.</param>
		/// <param name="mode">Interpolation mode.</param>
		public void MoveToPoint(Vector2 point, float time, InterpolationMode mode = InterpolationMode.Linear)
		{
			AnimateProperty(AnimatableProperty.Position, point, time, mode);
		}

		/// <summary>
		/// Move to a specified point in a given amount of time.
		/// </summary>
		/// <param name="x">Destination point's X coordinate.</param>
		/// <param name="y">Destination point's Y coordinate.</param>
		/// <param name="time">Movement duration.</param>
		/// <param name="mode">Interpolation mode.</param>
		public void MoveToPoint(float x, float y, float time, InterpolationMode mode = InterpolationMode.Linear)
		{
			AnimateProperty(AnimatableProperty.Position, new Vector2(x, y), time, mode);
		}

		/// <summary>
		/// Move an object along a bezier curve.
		/// </summary>
		/// <param name="points">Point array.</param>
		public void MoveAlongCurve(IEnumerable<Vector2> points)
		{
			GameCore.RegisterDeferredAction(() =>
				{
					Behaviours.Remove<BezierMovementBehaviour>();
					Behaviours.Add(new BezierMovementBehaviour(points));
				}
			);
		}

		/// <summary>
		/// Move an object along a bezier curve.
		/// </summary>
		/// <param name="points">Point array.</param>
		public void MoveAlongCurve(params Vector2[] points)
		{
			MoveAlongCurve(points as IEnumerable<Vector2>);
		}

		#endregion

		#region Animated Properties

		/// <summary>
		/// Animate a standard float property.
		/// </summary>
		/// <param name="pty">Property type to animate. Can be Angle, Scale and Transparency.</param>
		/// <param name="value">Desired value.</param>
		/// <param name="time">Effect duration.</param>
		/// <param name="interpolationMode">Interpolation mode.</param>
		/// <param name="animateBack">Flag that animates the property back when the effect has finished.</param>
		public void AnimateProperty(AnimatableProperty pty,
									float value,
									float time,
									InterpolationMode interpolationMode = InterpolationMode.Linear,
									bool animateBack = false)
		{
			if(pty == AnimatableProperty.Custom)
				throw new ArgumentException("Custom properties need a setter to be animated.");

			StopAnimatingProperty(pty);
			_AnimatedProperties.Add(new FloatPropertyAnimator(this, value, time, pty, interpolationMode) { AnimateBack = animateBack });
		}

		/// <summary>
		/// Animate a standard vector property.
		/// </summary>
		/// <param name="pty">Property to animate. Can be Position and ScaleVector.</param>
		/// <param name="value">Desired value.</param>
		/// <param name="time">Effect duration.</param>
		/// <param name="interpolationMode">Interpolation mode.</param>
		/// <param name="animateBack">Flag that animates the property back when the effect has finished.</param>
		public void AnimateProperty(AnimatableProperty pty,
									Vector2 value,
									float time,
									InterpolationMode interpolationMode = InterpolationMode.Linear,
									bool animateBack = false)
		{
			StopAnimatingProperty(pty);
			_AnimatedProperties.Add(new VectorPropertyAnimator(this, value, time, pty, interpolationMode) { AnimateBack = animateBack });
		}

		/// <summary>
		/// Animate a standard color property.
		/// </summary>
		/// <param name="value">Desired value.</param>
		/// <param name="time">Effect duration.</param>
		/// <param name="interpolationMode">Interpolation mode.</param>
		/// <param name="animateBack">Flag that animates the property back when the effect has finished.</param>
		public void AnimateProperty(Color value,
									float time,
									InterpolationMode interpolationMode = InterpolationMode.Linear,
									bool animateBack = false)
		{

			StopAnimatingProperty(AnimatableProperty.Color);
			_AnimatedProperties.Add(new ColorPropertyAnimator(this, value, time, AnimatableProperty.Color, interpolationMode) { AnimateBack = animateBack });
		}

		/// <summary>
		/// Animate a custom float property.
		/// </summary>
		/// <param name="setter">The function that sets the object's property.</param>
		/// <param name="initial">Initial value.</param>
		/// <param name="value">Desired value.</param>
		/// <param name="time">Effect duration.</param>
		/// <param name="interpolationMode">Interpolation mode.</param>
		/// <param name="animateBack">Flag that animates the property back when the effect has finished.</param>
		public void AnimateProperty(Action<DynamicObject,float> setter,
									float initial,
									float value,
									float time,
									InterpolationMode interpolationMode = InterpolationMode.Linear,
									bool animateBack = false)
		{
			_AnimatedProperties.Add(new CustomPropertyAnimator(initial, setter, value, time, interpolationMode) { AnimateBack = animateBack });
		}

		/// <summary>
		/// Check if a property is currently being animated.
		/// </summary>
		/// <param name="pty">Property type.</param>
		public bool IsAnimatingProperty(AnimatableProperty pty)
		{
			if(pty == AnimatableProperty.Custom)
				throw new ArgumentException("Custom property animations cannot be inspected.");

		    return findAnimator(pty) != null;
		}

        /// <summary>
        /// Suspend property animation until further notice.
        /// </summary>
        /// <param name="pty"></param>
        /// /// <param name="pause">Pause flag.</param>
        public void PauseAnimatingProperty(AnimatableProperty pty, bool pause = true)
        {
            var anim = findAnimator(pty);
            if (anim == null)
                return;

            anim.Paused = pause;
        }

        /// <summary>
        /// Suspend all property animation until further notice.
        /// </summary>
        /// <param name="pause">Pause flag.</param>
        public void PauseAnimatingAllProperties(bool pause = true)
        {
            Pause = pause 
                ? Pause | PauseTarget.PropertyAnimation
                : Pause ^ PauseTarget.PropertyAnimation;
        }

	    /// <summary>
		/// Stop animating a property.
		/// </summary>
		/// <param name="pty">Property.</param>
		/// <param name="finish">Set the property to desired value or leave as it is?</param>
		public void StopAnimatingProperty(AnimatableProperty pty, bool finish = true)
		{
		    var anim = findAnimator(pty);
		    if (anim == null)
		        return;

            if(finish)
                anim.SkipAnimation(this);

		    _AnimatedProperties.Remove(anim);
		}

		/// <summary>
		/// Stop animating all properties at once.
		/// </summary>
		/// <param name="finish">Set the property to desired value or leave it as it is?</param>
		public void StopAnimatingAllProperties(bool finish = true)
		{
			if(finish)
				foreach (var curr in _AnimatedProperties)
					curr.SkipAnimation(this);

			_AnimatedProperties.Clear();
		}

		/// <summary>
		/// Remove the object by applying fadeout effects.
		/// </summary>
		public override void Remove()
		{
			if (IsFadingOut)
				return;

			IsFadingOut = true;

			var effectExists = false;
			foreach (var curr in Behaviours)
			{
				var fadeOut = curr as IFadeOut;
				if (fadeOut != null)
				{
					// no break intended: activate ALL the effects! \o/
					fadeOut.ActivateFadeOut(this);
					effectExists = true;
				}
			}

			if(!effectExists)
				base.Remove();
		}

		/// <summary>
		/// Remove the object.
		/// </summary>
		/// <param name="instantly">Apply fadeout effects or not?</param>
		public void Remove(bool instantly)
		{
			if(instantly)
				base.Remove();
			else
				Remove();
		}

		/// <summary>
		/// Make the object blink.
		/// </summary>
		/// <param name="times">Number of times to blink.</param>
		/// <param name="duration">Length of the blinking effect duration.</param>
		public void Blink(int times, float duration)
		{
			Behaviours.Add(new BlinkingBehaviour(times, duration));
		}

        /// <summary>
        /// Find an animator for a property;
        /// </summary>
        /// <param name="pty">Animated property.</param>
        private IPropertyAnimator findAnimator(AnimatableProperty pty)
        {
            if(pty != AnimatableProperty.Custom)
                foreach (var curr in _AnimatedProperties)
                    if (curr.Property == pty)
                        return curr;

            return null;
        }

		/// <summary>
		/// Update all animated properties for the object.
		/// </summary>
		private void updateAnimatedProperties()
		{
			var count = _AnimatedProperties.Count;
			for (var idx = 0; idx < count; idx++)
			{
				var curr = _AnimatedProperties[idx];
			    if (curr.Paused)
			        continue;

				curr.UpdateProperty(this);

				// clean up finished animations to save space
				if (curr.Finished)
				{
					_AnimatedProperties.RemoveAt(idx);
					count--;
				}
			}
		}

		#endregion
	}


	/// <summary>
	/// The object parts that can be paused.
	/// </summary>
	[Flags]
	public enum PauseTarget
	{
		/// <summary>
		/// Movement of the object.
		/// </summary>
		Momentum = 0x01,

		/// <summary>
		/// Scaling, rotating and tinting.
		/// </summary>
		Transformation = 0x02,

		/// <summary>
		/// User-defined behaviours.
		/// </summary>
		Behaviours = 0x04,

		/// <summary>
		/// Animation of AnimatedSprite
		/// </summary>
		SpriteAnimation = 0x10,

		/// <summary>
		/// Animated properties.
		/// </summary>
		PropertyAnimation = 0x20,

		None = 0,
		Physics = Momentum | Transformation | Behaviours | PropertyAnimation,
        All = Physics | SpriteAnimation
	}
}
