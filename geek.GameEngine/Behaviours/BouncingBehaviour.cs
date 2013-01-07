using System;
using Microsoft.Xna.Framework;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The bouncing ball behaviour.
	/// </summary>
	public class BouncingBehaviour : IBehaviour
	{
		#region Constructors

		public BouncingBehaviour(bool absolute = false)
		{
			_Rect = GameCore.ScreenRect;
			_Absolute = absolute;
		}

		public BouncingBehaviour(Rectangle rect, bool absolute = false)
		{
			_Rect = rect;
			_Absolute = absolute;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The rectangle to bounce in.
		/// </summary>
		private Rectangle _Rect;

		/// <summary>
		/// Flag indicating whether the rectangle is in relative or absolute coordinates.
		/// </summary>
		private readonly bool _Absolute;

		#endregion

		public void UpdateObjectState(DynamicObject obj)
		{
			var iObj = obj as InteractableObject;
			if (iObj == null)
				return;

			var box = obj.GetBoundingBox(_Absolute);
			if (obj.Momentum.X < 0 && box.Left <= _Rect.Left)
				obj.Momentum.X = Math.Abs(obj.Momentum.X);
			else if (obj.Momentum.X > 0 && box.Right >= _Rect.Right)
				obj.Momentum.X = -Math.Abs(obj.Momentum.X);
			else if (obj.Momentum.Y < 0 && box.Top <= _Rect.Top)
				obj.Momentum.Y = Math.Abs(obj.Momentum.Y);
			else if (obj.Momentum.Y > 0 && box.Bottom >= _Rect.Bottom)
				obj.Momentum.Y = -Math.Abs(obj.Momentum.Y);
		}
	}
}
