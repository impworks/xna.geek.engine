using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using geek.GameEngine.Visuals;

namespace geek.GameEngine
{
	/// <summary>
	/// The class that manages touch events.
	/// </summary>
	public static class TouchManager
	{
		#region Constructors

		static TouchManager()
		{
			_RegisteredTouches = new Dictionary<int, InteractableObject>(4);
		}

		#endregion

		#region Fields

		/// <summary>
		/// The cache of touch locations.
		/// Prevents multiple collection enumeration interference.
		/// </summary>
		public static TouchCollection TouchLocationCache;

		/// <summary>
		/// The list of touches that have been registered.
		/// </summary>
		private static readonly Dictionary<int, InteractableObject> _RegisteredTouches;

		#endregion

		#region Methods

		/// <summary>
		/// Register a touch location as handled.
		/// </summary>
		/// <param name="id">Touch location id.</param>
		/// <param name="obj">Object to capture the touch.</param>
		static public void HandleTouch(int id, InteractableObject obj)
		{
			if(!_RegisteredTouches.ContainsKey(id))
				_RegisteredTouches.Add(id, obj);
		}

		/// <summary>
		/// Check if a touch location has been handled.
		/// </summary>
		/// <param name="id">Touch id.</param>
		/// <param name="obj">Object that has captured the touch.</param>
		/// <returns></returns>
		static public bool CanHandleTouch(int id, InteractableObject obj)
		{
			return !_RegisteredTouches.ContainsKey(id) || _RegisteredTouches[id] == obj;
		}

		/// <summary>
		/// Update the touch manager.
		/// </summary>
		static public void Update()
		{
			_RegisteredTouches.Clear();
			TouchLocationCache = TouchPanel.GetState();
		}

		#endregion
	}

	/// <summary>
	/// Touch modes for InteractableObject.
	/// </summary>
	[Flags]
	public enum TouchMode
	{
		/// <summary>
		/// Process one tap per each object.
		/// </summary>
		Default = 0x0,

		/// <summary>
		/// Make the object tap-transparent and do not mark it's taps as handled.
		/// </summary>
		TapThrough = 0x1,

		/// <summary>
		/// Process already handled taps as well.
		/// </summary>
		Shared = 0x2,
	}
}
