using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using geek.GameEngine.Sprites;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// The class that describes all active objects in the game that move, animate and interact with the user.
	/// </summary>
	public class GameObject : InteractableObject
	{
		#region Constructors

		public GameObject()
		{
			Animations = new List<SpriteBase>();
			CollisionDetectionMode = CollisionDetectionMode.Perfect;
		}

		public GameObject(Vector2 position)
			: this()
		{
			Position = position;
		}

		public GameObject(float x, float y)
			: this()
		{
			Position = new Vector2(x, y);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Activate blendable animations' blending mode?
		/// </summary>
		public bool IsBlendingEnabled;

		/// <summary>
		/// List of animations available for this object.
		/// </summary>
		public List<SpriteBase> Animations { get; private set; }

		/// <summary>
		/// The ID of the current animation.
		/// </summary>
		private int _CurrentAnimation;

		/// <summary>
		/// The collision detection mode.
		/// </summary>
		public CollisionDetectionMode CollisionDetectionMode;

		#endregion

		#region VisualObjectBase overrides

		/// <summary>
		/// Update the current animation.
		/// </summary>
		public override void Update()
		{
			base.Update();

			if ((Pause & PauseTarget.SpriteAnimation) == 0)
				GetCurrentAnimation().Update();
		}

		/// <summary>
		/// Draws the current animation frame to screen.
		/// </summary>
		/// <param name="batch">Sprite batch</param>
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);

			if(IsVisible)
				GetCurrentAnimation().Draw(batch);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Creates a new static animation frame for the object.
		/// </summary>
		/// <param name="assetName">Asset name to use.</param>
		/// <returns></returns>
		public Sprite AddAnimation(string assetName)
		{
			var sprite = new Sprite(this, ResourceCache.Get<Texture2D>(assetName));
			Animations.Add(sprite);
			return sprite;
		}

		/// <summary>
		/// Create a new animation for the object.
		/// </summary>
		/// <param name="assetName">Asset name to use as texture.</param>
		/// <param name="frameCount">Number of frames in the animation.</param>
		/// <param name="frameRate">Frame rate.</param>
		/// <param name="looped">Loop animation flag.</param>
		/// <returns></returns>
		public AnimatedSprite AddAnimation(string assetName, int frameCount, float frameRate, bool looped = true)
		{
			var sprite = new AnimatedSprite(this, ResourceCache.Get<Texture2D>(assetName), frameCount, frameRate) { Looped = looped };
			Animations.Add(sprite);
			return sprite;
		}

		/// <summary>
		/// Set the current animation and play it.
		/// </summary>
		/// <param name="id">Animation id.</param>
		public void SetAnimation(int id, bool restart = false)
		{
			if (id < 0 || id >= Animations.Count)
				throw new ArgumentOutOfRangeException("Animation id does not exist!");

			_CurrentAnimation = id;
			if(restart)
				Animations[_CurrentAnimation].Reset();
		}

		/// <summary>
		/// Set the current animation and play it.
		/// </summary>
		/// <param name="sprite">Animation.</param>
		public void SetAnimation(SpriteBase sprite)
		{
			var idx = Animations.IndexOf(sprite);
			if(idx == -1)
				throw new ArgumentException("Sprite does not exist in the list of animations!");

			_CurrentAnimation = idx;
			sprite.Reset();
		}

		/// <summary>
		/// Returns the current animation that is played.
		/// </summary>
		/// <returns></returns>
		public SpriteBase GetCurrentAnimation()
		{
			return Animations[_CurrentAnimation];
		}

		/// <summary>
		/// Returns the bounding box for the object. The current sprite is used.
		/// The bounding box does not take transparency into account.
		/// </summary>
		/// <returns></returns>
		public override Rectangle GetBoundingBox(bool absolute = false)
		{
			var size = GetCurrentAnimation().Size;
			var pos = GetPosition(absolute);
			var hotSpot = GetCurrentAnimation().HotSpot;
			if (ScaleVector.HasValue)
			{
				var left = pos.X - hotSpot.X*ScaleVector.Value.X;
				var top = pos.Y - hotSpot.Y*ScaleVector.Value.Y;
				return new Rectangle((int)left, (int)top, (int)(size.X * ScaleVector.Value.X), (int)(size.Y * ScaleVector.Value.Y));
			}
			else
			{
				var topLeft = pos - hotSpot*Scale;
				return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(size.X * Scale), (int)(size.Y * Scale));	
			}
		}

		#endregion

		#region Collision detection

		/// <summary>
		/// Smart collision detection allowing for pixel-perfect collisions.
		/// </summary>
		/// <param name="obj">Object to test against</param>
		/// <returns></returns>
		public override bool IsOverlappedWith(InteractableObject obj)
		{
			var box1 = GetBoundingBox(true);
			var box2 = obj.GetBoundingBox(true);
			var isect = Rectangle.Intersect(box1, box2);
			if (isect.IsEmpty)
				return false;

			var gameObject = obj as GameObject;

			// Check whether both objects are GameObjects and are neither rotated nor scaled
			if (CollisionDetectionMode == CollisionDetectionMode.Fast
				|| gameObject == null
				|| !Scale.IsAlmost(1)
				|| !obj.Scale.IsAlmost(1)
				|| ScaleVector.HasValue
				|| obj.ScaleVector.HasValue
				|| (!Angle.IsAlmostNull() && CollisionDetectionMode != CollisionDetectionMode.Round)
				|| (!gameObject.Angle.IsAlmostNull() && gameObject.CollisionDetectionMode != CollisionDetectionMode.Round)
			)
			return true;

			// Convert it from screen coordinates to texture coordinates
			Rectangle textureRect1 = isect, textureRect2 = isect;
			textureRect1.X -= box1.X;
			textureRect1.Y -= box1.Y;
			textureRect2.X -= box2.X;
			textureRect2.Y -= box2.Y;

			var colorData1 = GetCurrentAnimation().GetTextureRegion(textureRect1);
			var colorData2 = gameObject.GetCurrentAnimation().GetTextureRegion(textureRect2);

			// If both pixels in the same location are non-transparent
			// HACK: only 50% of the pixels are checked for the sake of speed
			for (var idx = 0; idx < colorData1.Length; idx += 2)
				if (colorData1[idx].A != 0 && colorData2[idx].A != 0)
					return true;

			return false;
		}

		#endregion
	}

	/// <summary>
	/// Collision detection mode.
	/// </summary>
	public enum CollisionDetectionMode
	{
		Fast,
		Round,
		Perfect
	}
}
