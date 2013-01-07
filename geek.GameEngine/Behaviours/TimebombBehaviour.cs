using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The time bomb behaviour makes the object destroy itself in a given amount of time.
	/// </summary>
	public class TimebombBehaviour : IBehaviour
	{
		#region Constructor

		public TimebombBehaviour(float timeToLive)
		{
			_TimeToLive = timeToLive;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Estimated time to live of the object.
		/// </summary>
		private float _TimeToLive;

		#endregion

		#region Methods

		public void UpdateObjectState(DynamicObject obj)
		{
			_TimeToLive -= GameCore.Delta;

			if (_TimeToLive < 0)
			{
				obj.Remove();
				GameCore.RegisterDeferredAction(() => obj.Behaviours.Remove<TimebombBehaviour>());
			}
		}

		#endregion
	}
}
