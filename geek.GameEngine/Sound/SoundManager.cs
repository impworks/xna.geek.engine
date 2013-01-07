using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace geek.GameEngine.Sound
{
	/// <summary>
	/// The class that handles sound playback.
	/// </summary>
	public static class SoundManager
	{
		#region Constructor

		static SoundManager()
		{
			_SoundCache = new Dictionary<string, SoundEffectInfo>();
		}

		#endregion

		#region Fields

		public static bool CanPlayMusic { get { return MediaPlayer.GameHasControl; } }

		private static bool _SoundEnabled;
		private static bool _MusicEnabled;

		/// <summary>
		/// Gets or sets sound playback availability.
		/// </summary>
		public static bool SoundEnabled
		{
			get { return _SoundEnabled; }
			set
			{
				_SoundEnabled = value;
				if (!value)
					StopAllSounds();
			}
		}

		/// <summary>
		/// Gets or sets background music availability.
		/// </summary>
		public static bool MusicEnabled
		{
			get { return _MusicEnabled; }
			set
			{
				_MusicEnabled = value;

				if (value)
					PlayMusic();
				else
					StopMusic();
			}
		}

		/// <summary>
		/// List of sound effect instances currently initialized.
		/// </summary>
		private static readonly Dictionary<string, SoundEffectInfo> _SoundCache;

		/// <summary>
		/// The music effect.
		/// </summary>
		private static Song _Music;

		#endregion

		#region Sounds

		/// <summary>
		/// Load a sound to the current
		/// </summary>
		/// <param name="assetName">Sound asset name.</param>
		public static void LoadSound(string assetName)
		{
			if (_SoundCache.ContainsKey(assetName))
				return;

			var effect = ResourceCache.Get<SoundEffect>(assetName);
			var sound = new SoundEffectInfo(assetName, effect);
			_SoundCache.Add(assetName, sound);
		}

		/// <summary>
		/// Play the sound.
		/// </summary>
		/// <param name="assetName">Sound effect's asset name.</param>
		/// <param name="allowOverlap">Whether many instances of the same sound can be played simultaneously or not.</param>
		/// <param name="volume">Volume of the sample (0..1).</param>
		public static void PlaySound(string assetName, bool allowOverlap = false, float volume = 1)
		{
			if (!SoundEnabled)
				return;

			LoadSound(assetName);
			var sound = _SoundCache[assetName];

			sound.Play(allowOverlap, volume);
		}

		/// <summary>
		/// Checks if sound is playing.
		/// </summary>
		/// <param name="assetName">Sound asset name.</param>
		/// <returns></returns>
		public static bool IsSoundPlaying(string assetName)
		{
			return _SoundCache.ContainsKey(assetName) && _SoundCache[assetName].IsPlaying;
		}

		/// <summary>
		/// Checks if any sound is playing.
		/// </summary>
		/// <returns></returns>
		public static bool IsAnySoundPlaying()
		{
			if (_SoundCache.Count > 0)
				foreach (var curr in _SoundCache.Values)
					if (curr.IsPlaying)
						return true;

			return false;
		}

		/// <summary>
		/// Stop a sound playing.
		/// </summary>
		/// <param name="assetName">Sound asset name.</param>
		public static void StopSound(string assetName)
		{
			if (!_SoundCache.ContainsKey(assetName))
				return;

			var sound = _SoundCache[assetName];
			sound.Stop();
		}

		/// <summary>
		/// Stop all sounds playing.
		/// </summary>
		public static void StopAllSounds()
		{
			foreach (var sound in _SoundCache.Values)
				sound.Stop();
		}

		#endregion

		#region Music

		/// <summary>
		/// Load the music.
		/// </summary>
		/// <param name="assetName">Music asset name.</param>
		public static void LoadMusic(string assetName)
		{
			_Music = GameCore.Content.Load<Song>(assetName);
		}

		/// <summary>
		/// Play a background music.
		/// </summary>
		public static void PlayMusic(bool force = false)
		{
			if (!MusicEnabled || _Music == null)
				return;

			if (!CanPlayMusic && !force)
				return;

			if(IsMusicPlaying() && !force)
				return;

			MediaPlayer.Play(_Music);
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 1;
		}

		/// <summary>
		/// Stop music playing.
		/// </summary>
		public static void StopMusic()
		{
			if (CanPlayMusic && IsMusicPlaying())
				MediaPlayer.Pause();
		}

		/// <summary>
		/// Checks if music is playing.
		/// </summary>
		/// <returns></returns>
		public static bool IsMusicPlaying()
		{
			return MediaPlayer.State == MediaState.Playing;
		}

		#endregion
	}
}
