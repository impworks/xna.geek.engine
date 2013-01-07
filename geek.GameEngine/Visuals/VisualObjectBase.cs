using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace geek.GameEngine.Visuals
{
	public abstract class VisualObjectBase
	{
		#region Constructors

		protected VisualObjectBase()
		{
			IsVisible = true;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Visual object position relative to it's parent (scene, batch, etc).
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// Base object, to which current object is relative.
		/// </summary>
		public ObjectGroup Parent { get; set; }

		/// <summary>
		/// Gets or sets the flag indicating the object is to be displayed.
		/// </summary>
		public bool IsVisible;

		/// <summary>
		/// The width of the object's bounding box.
		/// </summary>
		public float Width
		{
			get { return GetBoundingBox().Width; }
		}

		/// <summary>
		/// The height of the bounding box.
		/// </summary>
		public float Height
		{
			get { return GetBoundingBox().Height; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the absolute position of the sprite in scene coordinates.
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual Vector2 AbsolutePosition
		{
			get { return Parent == null ? Position : Parent.AbsolutePosition + Position; }
		}

		/// <summary>
		/// Returns the relative position of the sprite to the screen.
		/// This allows automatic scrolling.
		/// </summary>
		/// <returns></returns>
		public virtual Vector2 RelativePosition
		{
			get { return Parent == null ? Position : Parent.RelativePosition + Position; }
		}

		/// <summary>
		/// Get a position depending on the flag.
		/// </summary>
		/// <param name="absolute">Absolute or relative position.</param>
		/// <returns></returns>
		public Vector2 GetPosition(bool absolute = false)
		{
			return absolute ? AbsolutePosition : RelativePosition;
		}

		/// <summary>
		/// Get a bounding box for the current object.
		/// </summary>
		/// <param name="absolute">Absolute or relative coordinates?</param>
		/// <returns></returns>
		public virtual Rectangle GetBoundingBox(bool absolute = false)
		{
			var pos = GetPosition(absolute);
			return new Rectangle((int)pos.X, (int)pos.Y, 0, 0);
		}

		/// <summary>
		/// The update logic method.
		/// </summary>
		public abstract void Update();

		/// <summary>
		/// The update screen method.
		/// </summary>
		/// <param name="batch">SpriteBatch to draw to.</param>
		public abstract void Draw(SpriteBatch batch);

		#endregion

		#region Visual layering

		/// <summary>
		/// The layer ID.
		/// It is updated when any sprite is drawn, making layering depend on drawing order.
		/// The limit is 10K objects per redraw, which seems like a fair limit.
		/// </summary>
		private static float _LayerId;
		public static float LayerId { get { return _LayerId -= 0.00001f; } }

		/// <summary>
		/// Bring the object 1 layer down in the drawing order.
		/// </summary>
		public void BringDown()
		{
			List<VisualObjectBase> list;
			int index;
			var ok = getContainingObjectList(out list, out index);
			if (!ok || index == list.Count-1)
				return;

			GameCore.RegisterDeferredAction(() =>
				{
					var tmp = list[index];
					list[index] = list[index + 1];
					list[index + 1] = tmp;
				}
			);
		}

		/// <summary>
		/// Bring the object to the bottom of current object batch.
		/// </summary>
		public void BringToBack()
		{
			List<VisualObjectBase> list;
			int index;
			var ok = getContainingObjectList(out list, out index);
			if (!ok || index == list.Count - 1)
				return;

			GameCore.RegisterDeferredAction(() =>
				{
					list.Remove(this);
					list.Add(this);
				}
			);
		}

		/// <summary>
		/// Bring the object 1 layer up in the drawing order.
		/// </summary>
		public void BringUp()
		{
			List<VisualObjectBase> list;
			int index;
			var ok = getContainingObjectList(out list, out index);
			if (!ok || index == 0)
				return;

			GameCore.RegisterDeferredAction(() =>
				{
					var tmp = list[index];
					list[index] = list[index - 1];
					list[index - 1] = tmp;
				}
			);
		}

		/// <summary>
		/// Bring the object to the top of current object batch.
		/// </summary>
		public void BringToFront()
		{
			List<VisualObjectBase> list;
			int index;
			var ok = getContainingObjectList(out list, out index);
			if (!ok || index == 0)
				return;

			GameCore.RegisterDeferredAction(() =>
				{
					list.Remove(this);
					list.Insert(0, this);
				}
			);
		}

		/// <summary>
		/// Remove the object from visual list.
		/// </summary>
		public virtual void Remove()
		{
			List<VisualObjectBase> list;
			int index;
			var ok = getContainingObjectList(out list, out index);
			if (!ok)
				return;

			GameCore.RegisterDeferredAction(() => list.Remove(this));
		}

		/// <summary>
		/// Gets the parent VisualObjectBatch's list for further manipulations with layering.
		/// </summary>
		/// <param name="list">The list containing current object.</param>
		/// <param name="index">The index of the item in the list.</param>
		/// <returns>Success flag.</returns>
		private bool getContainingObjectList(out List<VisualObjectBase> list, out int index)
		{
			list = null;
			index = -1;

			if (Parent == null)
				return false;

			list = Parent.Objects;
			index = list.IndexOf(this);
			return index != -1;
		}

		/// <summary>
		/// Reset the layer id to start new drawing loop.
		/// </summary>
		public static void ResetLayerId()
		{
			_LayerId = 1f;
		}

		#endregion
	}
}
