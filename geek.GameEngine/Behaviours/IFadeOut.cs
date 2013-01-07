using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// A fade out behaviour will be activated on obj.Remove() call.
	/// </summary>
	public interface IFadeOut
	{
		/// <summary>
		/// Is invoked when obj.Remove() is called.
		/// Must activate the fade out behaviour.
		/// </summary>
		void ActivateFadeOut(DynamicObject obj);

		/// <summary>
		/// Checks if the object can be removed.
		/// </summary>
		bool FadeOutFinished { get; }
	}
}
