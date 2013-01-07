using geek.GameEngine.AnimatedProperties;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// Creates a fade in effect when applied to an object.
	/// </summary>
	public class FadeOutBehaviour : IBehaviour, IFadeOut
	{
		#region Constructor

		public FadeOutBehaviour(float time, FadeEffect effect = FadeEffect.Fade, InterpolationMode mode = InterpolationMode.Linear)
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
		private float _Time;

		/// <summary>
		/// Effect type.
		/// </summary>
		private FadeEffect _Effect;

		/// <summary>
		/// Effect easing mode.
		/// </summary>
		private InterpolationMode _Mode;

		/// <summary>
		/// Time elapsed since effect's activation.
		/// </summary>
		private float? _ElapsedTime;

		/// <summary>
		/// Checks whether the animation has been applied already.
		/// </summary>
		private bool _Applied;

		#endregion

		#region IBehaviour implementation

		public void UpdateObjectState(DynamicObject obj)
		{
			if (!_ElapsedTime.HasValue)
				return;

			if (!_Applied)
			{
				if (_Effect == FadeEffect.Fade || _Effect == FadeEffect.ZoomAndFade || _Effect == FadeEffect.InverseZoomAndFade)
					obj.AnimateProperty(AnimatableProperty.Transparency, 0, _Time, _Mode);

				if (_Effect == FadeEffect.Zoom || _Effect == FadeEffect.ZoomAndFade || _Effect == FadeEffect.InverseZoomAndFade)
					obj.AnimateProperty(AnimatableProperty.Scale, _Effect == FadeEffect.Zoom ? 0 : 2, _Time, _Mode);

				_Applied = true;
			}

			_ElapsedTime += GameCore.Delta;
		}

		#endregion

		#region IFadeOut implementation

		public void ActivateFadeOut(DynamicObject obj)
		{
			_ElapsedTime = 0;
		}

		public bool FadeOutFinished
		{
			get { return _ElapsedTime > _Time; }
		}

		#endregion
	}
}
