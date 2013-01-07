using System;
using System.Collections.Generic;
using geek.GameEngine.Visuals;

namespace geek.GameEngine
{
	/// <summary>
	/// Manages the game's storyboard and transitions between scenes.
	/// </summary>
	public static class GameStoryBoard
	{
		#region Constructors

		static GameStoryBoard()
		{
			Scenes = new Dictionary<string, GameScene>();
		}

		#endregion

		#region Fields

		/// <summary>
		/// The levels used in the game.
		/// </summary>
		public static Dictionary<string, GameScene> Scenes;

		/// <summary>
		/// Gets the current level.
		/// </summary>
		public static GameScene CurrentScene { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Create a new level and add it to the storyboard.
		/// </summary>
		/// <param name="name">Desired level name (must be unique across the game).</param>
		/// <param name="level">Level.</param>
		/// <returns></returns>
		public static void AddLevel(string name, GameScene level)
		{
			if(LevelExists(name))
				throw new ArgumentException("Level with such name already exists!");

			Scenes.Add(name, level);

			if (CurrentScene == null)
			{
				CurrentScene = level;
				level.OnLevelStarted();
			}
		}

		/// <summary>
		/// Go to a new level.
		/// </summary>
		/// <param name="name">Level name.</param>
		public static void GotoLevel(string name)
		{
			if (!LevelExists(name))
				throw new ArgumentException("Level with such name does not exist!");

			if(CurrentScene != null)
				CurrentScene.OnLevelEnded();

			CurrentScene = Scenes[name];
			CurrentScene.OnLevelStarted();
		}

		/// <summary>
		/// Restart the current level.
		/// </summary>
		public static void RestartLevel()
		{
			if(CurrentScene == null)
				throw new InvalidOperationException("No level has been selected!");

			CurrentScene.OnLevelEnded();
			CurrentScene.OnLevelStarted();
		}

		/// <summary>
		/// Checks whether a level with such name exists.
		/// </summary>
		/// <param name="name">Level name.</param>
		/// <returns></returns>
		public static bool LevelExists(string name)
		{
			return Scenes.ContainsKey(name);
		}

		/// <summary>
		/// Remove a specific level from storyboard.
		/// </summary>
		/// <param name="name">Level.</param>
		public static void RemoveLevel(string name)
		{
			if (Scenes.ContainsKey(name))
			{
				var scene = Scenes[name];
				if (scene == CurrentScene)
					CurrentScene = null;
				Scenes.Remove(name);
			}
		}

		#endregion
	}
}
