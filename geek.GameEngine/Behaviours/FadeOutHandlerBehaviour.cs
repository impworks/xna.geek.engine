using System;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// Invokes an arbitrary code on object's fadeout.
	/// </summary>
	public class FadeOutHandlerBehaviour : IBehaviour, IFadeOut
	{
		#region Constructors

		public FadeOutHandlerBehaviour(Action<DynamicObject> handler)
		{
			_Handler = handler;
		}

		public FadeOutHandlerBehaviour(Action handler)
		{
			_Handler = obj => handler();
		}

		#endregion

		#region Fields

		/// <summary>
		/// The code to execute when the object is removed.
		/// </summary>
		private readonly Action<DynamicObject> _Handler;

		#endregion

		#region IBehaviour implementation

		public void UpdateObjectState(DynamicObject obj) { }

		#endregion

		#region IFadeOut implementation
		
		public void ActivateFadeOut(DynamicObject obj)
		{
			_Handler(obj);
		}

		public bool FadeOutFinished
		{
			get { return true; }
		}

		#endregion
	}
}
