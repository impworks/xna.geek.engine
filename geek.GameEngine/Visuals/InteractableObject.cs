using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using geek.GameEngine.Utils;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// The class that defines an object that can test for collisions and UI interaction.
	/// </summary>
	public abstract class InteractableObject : DynamicObject
	{
		#region Constructors

		protected InteractableObject()
		{
			IsTouchable = true;
			TouchMode = TouchMode.Default;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Enables or disables bounding box drawing.
		/// </summary>
		public bool DebugOutput;

		/// <summary>
		/// Enables or disables touch panel querying.
		/// </summary>
		public bool IsTouchable;

		/// <summary>
		/// Gets or sets the touch handling mode.
		/// </summary>
		public TouchMode TouchMode;

		/// <summary>
		/// Gets the touch location.
		/// </summary>
		public TouchLocation? Touch { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Check if the object has been tapped.
		/// Fires once per tap & hold.
		/// </summary>
		public bool IsTapped()
		{
			return IsPressed() && Touch.Value.State == TouchLocationState.Pressed;
		}

		/// <summary>
		/// Check if the user has released the tap on the object.
		/// Fires once per release.
		/// </summary>
		/// <returns></returns>
		public bool IsClicked()
		{
			return IsPressed() && Touch.Value.State == TouchLocationState.Released;
		}

		/// <summary>
		/// Check if the user has pressed the object with their finger.
		/// Fires continuosly until the finger is released.
		/// </summary>
		public bool IsPressed()
		{
			if (!IsTouchable || !Touch.HasValue)
				return false;

			var unhandled = TouchManager.CanHandleTouch(Touch.Value.Id, this);
			if (unhandled && TouchMode.HasFlag(TouchMode.TapThrough))
				TouchManager.HandleTouch(Touch.Value.Id, this);

			return unhandled || TouchMode.HasFlag(TouchMode.Shared);
		}

		/// <summary>
		/// Check whether an object collides with another object.
		/// Collision is rectangle based and non-rotatable.
		/// </summary>
		/// <param name="obj">Object to test collision to.</param>
		public virtual bool IsOverlappedWith(InteractableObject obj)
		{
			return GetBoundingBox(true).Intersects(obj.GetBoundingBox(true));
		}

		/// <summary>
		/// Checks whether the object is fully inside the screen.
		/// </summary>
		public bool IsOnScreen()
		{
			return IsInsideRect(GameCore.ScreenRect, false);
		}

		/// <summary>
		/// Checks whether the object is fully invisible.
		/// </summary>
		/// <returns></returns>
		public bool IsOffScreen()
		{
			return IsOutsideRect(GameCore.ScreenRect, false);
		}

		/// <summary>
		/// Checks whether the object is fully inside the playfield of current level.
		/// </summary>
		public bool IsOnPlayfield()
		{
			return IsInsideRect(GameStoryBoard.CurrentScene.LevelRect, true);
		}

		/// <summary>
		/// Checks whether the object is fully outside the playfield of current level.
		/// </summary>
		public bool IsOffPlayfield()
		{
			return IsOutsideRect(GameStoryBoard.CurrentScene.LevelRect, true);
		}

		/// <summary>
		/// Checks if the object has just entered the screen.
		/// The object must enter it naturally using it's momentum.
		/// </summary>
		/// <param name="side">Side of the screen.</param>
		/// <returns></returns>
		public bool EntersScreen(RectSide side)
		{
			return EntersRect(side, GameCore.ScreenRect, false);
		}

		/// <summary>
		/// Checks if the object has just left the screen.
		/// The object must leave it naturally using it's momentum.
		/// </summary>
		/// <param name="side">Side of the screen.</param>
		/// <returns></returns>
		public bool LeavesScreen(RectSide side)
		{
			return LeavesRect(side, GameCore.ScreenRect, false);
		}

		/// <summary>
		/// Checks if the object has just entered the playfield.
		/// The object must enter it naturally using it's momentum.
		/// </summary>
		/// <param name="side">Side of the screen.</param>
		/// <returns></returns>
		public bool EntersPlayfield(RectSide side)
		{
			return EntersRect(side, GameStoryBoard.CurrentScene.LevelRect, true);
		}

		/// <summary>
		/// Checks if the object has just left the playfield.
		/// The object must leave it naturally using it's momentum.
		/// </summary>
		/// <param name="side">Side of the playfield.</param>
		/// <returns></returns>
		public bool LeavesPlayfield(RectSide side)
		{
			return LeavesRect(side, GameStoryBoard.CurrentScene.LevelRect, true);
		}

		/// <summary>
		/// Checks whether the object enters a rectangle.
		/// </summary>
		/// <param name="side">Rectangle side.</param>
		/// <param name="rect">Rectangle.</param>
		/// <param name="absolute">Flag indicating whether rectangle coordinates are screen-bound or playfield-bound.</param>
		/// <returns></returns>
		public bool EntersRect(RectSide side, Rectangle rect, bool absolute)
		{
			var box = GetBoundingBox(absolute);

			return (side.HasFlag(RectSide.Top) && entersRectFromTop(rect, box))
				|| (side.HasFlag(RectSide.Left) && entersRectFromLeft(rect, box))
				|| (side.HasFlag(RectSide.Right) && entersRectFromRight(rect, box))
				|| (side.HasFlag(RectSide.Bottom) && entersRectFromBottom(rect, box));
		}

		/// <summary>
		/// Checks whether the object leaves a rectangle.
		/// </summary>
		/// <param name="side">Rectangle side.</param>
		/// <param name="rect">Rectangle.</param>
		/// <param name="absolute">Flag indicating whether rectangle coordinates are screen-bound or playfield-bound.</param>
		/// <returns></returns>
		public bool LeavesRect(RectSide side, Rectangle rect, bool absolute)
		{
			var box = GetBoundingBox(absolute);

			return (side.HasFlag(RectSide.Top) && leavesRectFromTop(rect, box))
				|| (side.HasFlag(RectSide.Left) && leavesRectFromLeft(rect, box))
				|| (side.HasFlag(RectSide.Right) && leavesRectFromRight(rect, box))
				|| (side.HasFlag(RectSide.Bottom) && leavesRectFromBottom(rect, box));
		}

		/// <summary>
		/// Find a touch location that matches the current object.
		/// </summary>
		/// <param name="zone"></param>
		/// <returns></returns>
		private static TouchLocation? findTouch(Rectangle zone)
		{
			foreach(var currTouch in TouchManager.TouchLocationCache)
			{
				if (currTouch.State == TouchLocationState.Invalid)
					continue;

				if(GameCore.Orientation == DisplayOrientation.LandscapeLeft)
					if (zone.Contains((int)currTouch.Position.X, (int)currTouch.Position.Y))
						return currTouch;

				if (GameCore.Orientation == DisplayOrientation.Portrait)
					if (zone.Contains((int)(GameCore.ScreenSize.X - currTouch.Position.Y), (int)currTouch.Position.X))
						return currTouch;
			}

			return null;
		}

		#endregion

		#region VisualObjectBase implementation

		public override void Update()
		{
			base.Update();

			if (IsTouchable)
				Touch = findTouch(GetBoundingBox());
		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			if (DebugOutput)
			{ 
				drawBoundingBox();
				drawHotSpot();
			}
		}

		#endregion

		#region Geometric methods

		/// <summary>
		/// Checks whether this object is inside a rectangle.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="absolute">Absolute or relative coordinates.</param>
		/// <returns></returns>
		public bool IsInsideRect(Rectangle rect, bool absolute = false)
		{
			return rect.Contains(GetBoundingBox(absolute));
		}

		/// <summary>
		/// Checks whether this object is inside a rectangle.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="absolute">Absolute or relative coordinates.</param>
		/// <returns></returns>
		public bool IsOutsideRect(Rectangle rect, bool absolute = false)
		{
			return !rect.Intersects(GetBoundingBox(absolute));
		}

		/// <summary>
		/// Checks if the object enters a specific rectangle from the top.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool entersRectFromTop(Rectangle rect, Rectangle box)
		{
			var line = rect.Top;
			var otherAxisOk = box.Left > rect.Left || box.Right < rect.Right;
			return otherAxisOk && box.Bottom >= line && box.Top < line && Momentum.Y > 0;
		}

		/// <summary>
		/// Checks if the object enters a specific rectangle from the left.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool entersRectFromLeft(Rectangle rect, Rectangle box)
		{
			var line = rect.Left;
			var otherAxisOk = box.Top > rect.Top || box.Bottom < rect.Bottom;
			return otherAxisOk && box.Right >= line && box.Left < line && Momentum.X > 0;
		}

		/// <summary>
		/// Checks if the object enters a specific rectangle from the right.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool entersRectFromRight(Rectangle rect, Rectangle box)
		{
			var line = rect.Right;
			var otherAxisOk = box.Top > rect.Top || box.Bottom < rect.Bottom;
			return otherAxisOk && box.Left <= line && box.Right > line && Momentum.X < 0;
		}

		/// <summary>
		/// Checks if the object enters a specific rectangle from the bottom.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool entersRectFromBottom(Rectangle rect, Rectangle box)
		{
			var line = rect.Bottom;
			var otherAxisOk = box.Left > rect.Left || box.Right < rect.Right;
			return otherAxisOk && box.Top <= line && box.Bottom > line && Momentum.Y < 0;
		}

		/// <summary>
		/// Checks if the object leaves a specific rectangle to the top.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool leavesRectFromTop(Rectangle rect, Rectangle box)
		{
			var line = rect.Top;
			var otherAxisOk = box.Left > rect.Left || box.Right < rect.Right;
			return otherAxisOk && box.Top <= line && box.Bottom > line && Momentum.Y < 0;
		}

		/// <summary>
		/// Checks if the object leaves a specific rectangle to the left.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool leavesRectFromLeft(Rectangle rect, Rectangle box)
		{
			var line = rect.Left;
			var otherAxisOk = box.Top > rect.Top || box.Bottom < rect.Bottom;
			return otherAxisOk && box.Left <= line && box.Right > line && Momentum.X < 0;
		}

		/// <summary>
		/// Checks if the object leaves a specific rectangle to the right.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool leavesRectFromRight(Rectangle rect, Rectangle box)
		{
			var line = rect.Right;
			var otherAxisOk = box.Top > rect.Top || box.Bottom < rect.Bottom;
			return otherAxisOk && box.Right >= line && box.Left < line && Momentum.X > 0;
		}

		/// <summary>
		/// Checks if the object leaves a specific rectangle to the bottom.
		/// </summary>
		/// <param name="rect">Rectangle to test.</param>
		/// <param name="box">Bounding box.</param>
		/// <returns></returns>
		private bool leavesRectFromBottom(Rectangle rect, Rectangle box)
		{
			var line = rect.Bottom;
			var otherAxisOk = box.Left > rect.Left || box.Right < rect.Right;
			return otherAxisOk && box.Bottom >= line && box.Top < line && Momentum.Y > 0;
		}

		/// <summary>
		/// Draws the bounding box on screen.
		/// </summary>
		private void drawBoundingBox()
		{
			var box = GetBoundingBox();
			GameCore.DrawLine(box.Left, box.Top, box.Right, box.Top);
			GameCore.DrawLine(box.Right, box.Top, box.Right, box.Bottom);
			GameCore.DrawLine(box.Left, box.Bottom, box.Right, box.Bottom);
			GameCore.DrawLine(box.Left, box.Top, box.Left, box.Bottom);
		}

		/// <summary>
		/// Draws the cross where the hot spot of the object is located.
		/// </summary>
		private void drawHotSpot()
		{
			var pos = GetPosition(true);
			GameCore.DrawLine(pos.X - 5, pos.Y, pos.X + 5, pos.Y);
			GameCore.DrawLine(pos.X, pos.Y - 5, pos.X, pos.Y + 5);
		}

		#endregion
	}
}
