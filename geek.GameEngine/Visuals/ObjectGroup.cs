using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// A class that lets moving several visual objects at once.
	/// </summary>
	public class ObjectGroup : InteractableObject, IEnumerable<VisualObjectBase>
	{
		#region Constructors

		public ObjectGroup()
		{
			IsTouchable = false;
			Objects = new List<VisualObjectBase>();
		}

		public ObjectGroup(Vector2 position)
			: this()
		{
			Position = position;
		}

		public ObjectGroup(float x, float y)
			: this()
		{
			Position = new Vector2(x, y);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Flag indicating that the transparency of all objects within the grid is bound to grid's own transparency.
		/// </summary>
		public bool BindTransparency;

		/// <summary>
		/// List of objects managed by the group.
		/// </summary>
		public List<VisualObjectBase> Objects { get; protected set; }

		/// <summary>
		/// The reversed list of objects that is draw-order-friendly.
		/// </summary>
		public IEnumerable<VisualObjectBase> DrawOrderedObjects
		{
			get
			{
				for (var idx = Objects.Count - 1; idx >= 0; idx--)
					yield return Objects[idx];
			}
		}

		/// <summary>
		/// The shortcut to the number of objects in the list.
		/// </summary>
		public int Count
		{
			get { return Objects.Count; }
		}

		/// <summary>
		/// The shortcut to a particular item in the list.
		/// </summary>
		/// <param name="id">Item's ID.</param>
		/// <returns></returns>
		public virtual VisualObjectBase this[int id]
		{
			get { return Objects[id]; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add an object to the visual list.
		/// </summary>
		/// <param name="obj">Object to insert.</param>
		/// <param name="toTop">Whether to put object on top or on bottom.</param>
		public virtual void AddChild(VisualObjectBase obj, bool toTop = true)
		{
			if (obj == null || Objects.Contains(obj))
				return;

			if (obj.Parent != null)
				obj.Parent.Objects.Remove(obj);

			if (toTop)
				Objects.Insert(0, obj);
			else
				Objects.Add(obj);

			obj.Parent = this;
		}

		/// <summary>
		/// Add an object to the visual list in deferred actions stage.
		/// </summary>
		/// <param name="obj">Object to insert.</param>
		/// <param name="toTop">Whether to put object on top or on bottom.</param>
		public void AddChildDeferred(VisualObjectBase obj, bool toTop = true)
		{
			GameCore.RegisterDeferredAction(() => AddChild(obj, toTop));
		}

		/// <summary>
		/// Adds many objects to the visual list.
		/// </summary>
		/// <param name="objs">Objects to insert.</param>
		public void AddChildren(params VisualObjectBase[] objs)
		{
			for(var i = 0; i < objs.Length; i++)
				AddChild(objs[i]);
		}

		/// <summary>
		/// Adds many objects to the visual list in deferred actions stage.
		/// </summary>
		/// <param name="objs">Objects to insert.</param>
		public void AddChildrenDeferred(params VisualObjectBase[] objs)
		{
			GameCore.RegisterDeferredAction(() => AddChildren(objs));
		}

		/// <summary>
		/// Adds many objects to the visual list.
		/// </summary>
		/// <param name="toTop">Whether to put objects on top or on bottom.</param>
		/// <param name="objs">Objects to insert.</param>
		public void AddChildren(bool toTop, params VisualObjectBase[] objs)
		{
			for (var i = 0; i < objs.Length; i++)
				AddChild(objs[i], toTop);
		}

		/// <summary>
		/// Remove the object from the list.
		/// </summary>
		/// <param name="obj">Object to remove.</param>
		public void RemoveChild(VisualObjectBase obj)
		{
			Objects.Remove(obj);
		}

		/// <summary>
		/// Remove the object from the list.
		/// </summary>
		/// <param name="idx">Index of the object to remove.</param>
		public virtual void RemoveChildAt(int idx)
		{
			Objects.RemoveAt(idx);
		}

		/// <summary>
		/// Remove all the items from the list.
		/// </summary>
		public void Clear()
		{
			Objects.Clear();
		}

		#endregion

		#region VisualObjectBase overrides

		/// <summary>
		/// Update all sub-items inside the batch.
		/// </summary>
		public override void Update()
		{
			base.Update();

			foreach (var curr in Objects)
			{
				if (BindTransparency)
				{
					var dyn = curr as DynamicObject;
					if (dyn != null)
						dyn.Transparency = Transparency;
				}

				curr.Update();
			}
		}

		/// <summary>
		/// Redraw all the items inside the batch.
		/// </summary>
		/// <param name="batch">Sprite batch.</param>
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			if (!IsVisible)
				return;

			for (var idx = Objects.Count - 1; idx >= 0; idx--)
				Objects[idx].Draw(batch);
		}

		/// <summary>
		/// Get a box that contains all the group's objects.
		/// </summary>
		/// <param name="absolute">Alsolute or screen-relative coordinates?</param>
		/// <returns></returns>
		public override Rectangle GetBoundingBox(bool absolute = false)
		{
			if(Objects.Count == 0)
				return base.GetBoundingBox(absolute);

			var minx = float.PositiveInfinity;
			var miny = float.PositiveInfinity;
			var maxx = float.NegativeInfinity;
			var maxy = float.NegativeInfinity;

			foreach (var curr in Objects)
			{
				var box = curr.GetBoundingBox(absolute);
				if (box.X < minx) minx = box.X;
				if (box.Y < miny) miny = box.Y;
				if (box.Right > maxx) maxx = box.Right;
				if (box.Bottom > maxy) maxy = box.Bottom;
			}

			return new Rectangle((int)minx, (int)miny, (int)(maxx - minx), (int)(maxy - miny));
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<VisualObjectBase> GetEnumerator()
		{
			return (Objects as IEnumerable<VisualObjectBase>).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Objects.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// The generic group of objects that enforces all objects be of a specific class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TypedObjectGroup<T>: ObjectGroup, IEnumerable<T> where T: VisualObjectBase
	{
		#region Constructors

		public TypedObjectGroup()
		{}

		public TypedObjectGroup(Vector2 position)
			: this()
		{
			Position = position;
		}

		public TypedObjectGroup(float x, float y)
			: this()
		{
			Position = new Vector2(x, y);
		}

		#endregion

		#region Fields

		public new T this[int id]
		{
			get { return (T)Objects[id]; }
		}

		#endregion

		#region Methods

		[Obsolete("Use AddChild(T) method for typed group!", true)]
		public override void AddChild(VisualObjectBase obj, bool toTop = true)
		{
			if (obj is T)
				base.AddChild(obj, toTop);
			else
				throw new ArgumentException(string.Format("Object of type {0} cannot be put into a TypedObjectGroup<{1}>!", obj.GetType(), typeof(T)));
		}

		public void AddChild(T obj, bool toTop = true)
		{
			base.AddChild(obj, toTop);
		}

		#endregion

		#region IEnumerable<T> implementation

		public new IEnumerator<T> GetEnumerator()
		{
			foreach (var curr in Objects)
				yield return (T)curr;
		}

		#endregion
	}
}
