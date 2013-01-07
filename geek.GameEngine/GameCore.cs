using System;
using System.Collections.Generic;
using System.Diagnostics;
using geek.GameEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace geek.GameEngine
{
	/// <summary>
	/// The main class that governs all inner workings of geek.GameEngine.
	/// </summary>
	public static class GameCore
	{
		#region Initialization

		/// <summary>
		/// Set graphics mode for the game.
		/// </summary>
		/// <param name="game">Game instance.</param>
		public static void SetupGraphics(Game game)
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(game)
			{
				IsFullScreen = true,
				SynchronizeWithVerticalRetrace = true
			};
		}

		/// <summary>
		/// Saves the content pointer.
		/// </summary>
		/// <param name="content"></param>
		public static void LoadContent(ContentManager content)
		{
			FrameworkDispatcher.Update();

			if (content != null)
				Content = content;
		}

		/// <summary>
		/// Initialize the game engine for pure XNA application.
		/// </summary>
		/// <param name="game">Game instance.</param>
		public static void Init(Game game)
		{
			GraphicsDevice = game.GraphicsDevice;
			LoadContent(game.Content);

			initCommon();
		}

		/// <summary>
		/// Initialize the game engine for silverlight-xna shared application.
		/// </summary>
		public static void Init(GraphicsDevice device, ContentManager content)
		{
			GraphicsDevice = device;
			LoadContent(content);

			initCommon();
		}

		/// <summary>
		/// Common initialization features that are not project-type-dependent.
		/// </summary>
		private static void initCommon()
		{
			Orientation = DisplayOrientation.Portrait;

			_RenderTarget = new RenderTarget2D(GraphicsDevice, (int)ScreenSize.X, (int)ScreenSize.Y);
			_SpriteBatch = new SpriteBatch(GraphicsDevice);

			_BoundingBoxTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			_BoundingBoxTexture.SetData(new[] { Color.Red });

			_DeferredActions = new List<Action>();

			_BlendState = BlendState.AlphaBlend;
			SamplerState = SamplerState.LinearClamp;

			_TimedActions = new Timeline();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Graphics device to draw to.
		/// </summary>
		public static GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		/// Content manager.
		/// </summary>
		public static ContentManager Content { get; private set; }
		
		/// <summary>
		/// Gets the graphics device manager.
		/// </summary>
		public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

		/// <summary>
		/// Gets or sets the pause flag.
		/// </summary>
		public static bool IsPaused
		{
			get { return _IsPaused; }
			set
			{
				_IsPaused = value;
				AccelManager.Enabled = value;
			}
		}
		private static bool _IsPaused;

		/// <summary>
		/// Outputs FPS into debug console.
		/// </summary>
		public static bool OutputFPS;

		/// <summary>
		/// Gets or sets desired game orientation.
		/// </summary>
		private static DisplayOrientation _DisplayOrientation;
		public static DisplayOrientation Orientation
		{
			get { return _DisplayOrientation; }
			set
			{
				_DisplayOrientation = value;
				updateScreenSize(value);
			}
		}

		/// <summary>
		/// Gets the screen size in pixels.
		/// </summary>
		public static Vector2 ScreenSize { get; private set; }

		/// <summary>
		/// Gets the screen rectangle.
		/// </summary>
		public static Rectangle ScreenRect { get; private set; }

		/// <summary>
		/// The sampler state to use by default.
		/// </summary>
		public static SamplerState SamplerState { get; set; }

		/// <summary>
		/// Current delta value.
		/// </summary>
		public static float Delta { get; private set; }

		/// <summary>
		/// RenderTarget used to rotate level into portrait orientation.
		/// </summary>
		private static RenderTarget2D _RenderTarget;

		/// <summary>
		/// Sprite batch to draw to.
		/// </summary>
		private static SpriteBatch _SpriteBatch;

		/// <summary>
		/// Gets the texture for drawing a bounding box around objects in debug mode.
		/// </summary>
		private static Texture2D _BoundingBoxTexture;

		/// <summary>
		/// Current blending state of the sprite batch.
		/// </summary>
		private static BlendState _BlendState;

		/// <summary>
		/// Time elapsed since last FPS update.
		/// </summary>
		private static float _FpsElapsedTime;

		/// <summary>
		/// Frame count since last update.
		/// </summary>
		private static int _Fps;

		/// <summary>
		/// The list of deferred actions.
		/// </summary>
		private static List<Action> _DeferredActions;

		/// <summary>
		/// Timeline for timed actions.
		/// </summary>
		private static Timeline _TimedActions;

		#endregion

		#region Main Methods: Update and Draw

		/// <summary>
		/// The main Update method for all game components.
		/// </summary>
		/// <param name="delta"></param>
		public static void Update(float delta)
		{
			Delta = delta;
			validateStoryBoard();

			TouchManager.Update();
			_TimedActions.Update();
			GameStoryBoard.CurrentScene.Update();

			executeDeferredActions();
		}

		/// <summary>
		/// The main Draw method for all game components.
		/// </summary>
		/// <param name="delta"></param>
		public static void Draw(float delta)
		{
			Delta = delta;
			validateStoryBoard();
			
			// fps counter
			_Fps++;
			_FpsElapsedTime += delta;
			if (_FpsElapsedTime > 1)
			{
				if (_Fps >= 40)
					Debug.WriteLine("FPS: {0}", _Fps);
				else
					Debug.WriteLine("!!! FPS: {0}", _Fps);

				_Fps = 0;
				_FpsElapsedTime = 0;
			}

			Visuals.VisualObjectBase.ResetLayerId();

			if(Orientation == DisplayOrientation.Portrait)
				GraphicsDevice.SetRenderTarget(_RenderTarget);

			GraphicsDevice.Clear(GameStoryBoard.CurrentScene.Background);

			InitSpriteBatch();
			GameStoryBoard.CurrentScene.Draw(_SpriteBatch);
			_SpriteBatch.End();

			if (Orientation == DisplayOrientation.Portrait)
			{
				GraphicsDevice.SetRenderTarget(null);
				_SpriteBatch.Begin();
				_SpriteBatch.Draw(_RenderTarget, new Vector2(0, ScreenSize.X), null, Color.White, -MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);
				_SpriteBatch.End();
			}
		}

		#endregion

		#region Drawline

		/// <summary>
		/// Draws a white line on the screen.
		/// Is used for bounding box debugging.
		/// </summary>
		/// <param name="x1">First point's X coordinate.</param>
		/// <param name="y1">First point's Y coordinate.</param>
		/// <param name="x2">Second point's X coordinate.</param>
		/// <param name="y2">Second point's Y coordinate.</param>
		public static void DrawLine(float x1, float y1, float x2, float y2)
		{
			DrawLine(new Vector2(x1, y1), new Vector2(x2, y2));
		}

		/// <summary>
		/// Draws a white line on the screen.
		/// Is used for bounding box debugging.
		/// </summary>
		/// <param name="from">First point.</param>
		/// <param name="to">Second point.</param>
		public static void DrawLine(Vector2 from, Vector2 to)
		{
			var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
			var length = Vector2.Distance(from, to);

			_SpriteBatch.Draw(_BoundingBoxTexture, from, null, Color.White, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
		}

		#endregion

		#region Sprite batch methods

		/// <summary>
		/// Init the spriteBatch's default mode.
		/// </summary>
		public static void InitSpriteBatch()
		{
			_SpriteBatch.Begin(SpriteSortMode.BackToFront, _BlendState, SamplerState, DepthStencilState.None, RasterizerState.CullNone);
		}

		/// <summary>
		/// Update the blending state of the sprite batch.
		/// </summary>
		/// <param name="newState">New blending state.</param>
		public static void UpdateBlendingState(BlendState newState)
		{
			if (_BlendState == newState)
				return;

			_SpriteBatch.End();

			_BlendState = newState;
			InitSpriteBatch();
		}

		#endregion

		#region Deferred and timed action list

		/// <summary>
		/// Register a callback that is invoked after all objects are updated.
		/// It's used to manipulate object's position in the list to avoid modifying the collection being traversed.
		/// </summary>
		/// <param name="action"></param>
		public static void RegisterDeferredAction(Action action)
		{
			_DeferredActions.Add(action);
		}

		/// <summary>
		/// Execute all the deferred actions.
		/// </summary>
		private static void executeDeferredActions()
		{
			var actions = _DeferredActions;
			_DeferredActions = new List<Action>();

			foreach (var action in actions)
				action();
		}

		/// <summary>
		/// Register an action to occur after the given amount of time.
		/// </summary>
		/// <param name="timeout">Time to wait before executing the action.</param>
		/// <param name="action">Action to execute.</param>
		/// <param name="comment">Optional comment for debugging.</param>
		public static int RegisterTimedAction(float timeout, Action action, string comment = null)
		{
			return _TimedActions.Add(timeout, action, comment);
		}

		/// <summary>
		/// Remove the timed action from the timeline.
		/// </summary>
		/// <param name="id">Timed action's ID.</param>
		public static void CancelTimedAction(int id)
		{
			_TimedActions.Remove(id);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Check if the storyboard contains at least one level.
		/// </summary>
		private static void validateStoryBoard()
		{
			if (GameStoryBoard.CurrentScene == null)
				throw new InvalidOperationException("The game must contain at least one level!");
		}

		/// <summary>
		/// Update screen size depending on orientation
		/// </summary>
		/// <param name="orientation">Orientation mode.</param>
		private static void updateScreenSize(DisplayOrientation orientation)
		{
			var vp = GraphicsDevice.Viewport;

			ScreenSize = orientation == DisplayOrientation.Portrait
				? new Vector2(vp.Height, vp.Width)
				: new Vector2(vp.Width, vp.Height);

			ScreenRect = new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y);
		}

		#endregion
	}
}
