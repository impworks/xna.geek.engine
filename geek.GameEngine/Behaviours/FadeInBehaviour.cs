using geek.GameEngine.AnimatedProperties;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// Creates a fade in effect when applied to an object.
	/// </summary>
	public class FadeInBehaviour : IBehaviour
	{
		#region Constructor

		public FadeInBehaviour(float time, FadeEffect effect = FadeEffect.Fade, InterpolationMode mode = InterpolationMode.Linear)
		{
			_Time = time;
			_Effect = effect;
			_Mode = mode;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Effect duration.
		/// </summary>
		private readonly float _Time;

		/// <summary>
		/// Effect type.
		/// </summary>
		private readonly FadeEffect _Effect;

		/// <summary>
		/// Effect easing mode.
		/// </summary>
		private readonly InterpolationMode _Mode;

		/// <summary>
		/// The initial scale to fade the object to.
		/// </summary>
		private float? _InitialScale;

		/// <summary>
		/// The initial transparency to fade the object to.
		/// </summary>
		private float? _InitialTransparency;

		#endregion

		#region Methods

		public void UpdateObjectState(DynamicObject obj)
		{
			if (!_InitialScale.HasValue)
			{
				_InitialScale = obj.Scale;
				_InitialTransparency = obj.Transparency;
			}

			if (_Effect == FadeEffect.Fade || _Effect == FadeEffect.ZoomAndFade || _Effect == FadeEffect.InverseZoomAndFade)
			{
				obj.Transparency = 0;
				obj.AnimateProperty(AnimatableProperty.Transparency, _InitialTransparency.Value, _Time, _Mode);
			}

			if (_Effect == FadeEffect.Zoom || _Effect == FadeEffect.ZoomAndFade || _Effect == FadeEffect.InverseZoomAndFade)
			{
				obj.Scale = _Effect == FadeEffect.InverseZoomAndFade ? 2 : 0;
				obj.AnimateProperty(AnimatableProperty.Scale, _InitialScale.Value, _Time, _Mode);
			}

			GameCore.RegisterDeferredAction(() => obj.Behaviours.Remove<FadeInBehaviour>());
		}

		#endregion
	}
}
