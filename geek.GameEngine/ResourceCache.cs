using System;
using System.Collections.Generic;

namespace geek.GameEngine
{
	/// <summary>
	/// The asset cache.
	/// </summary>
	public static class ResourceCache
	{
		#region Constructor

		static ResourceCache()
		{
			_Cache = new Dictionary<string, object>();
		}

		#endregion

		#region Fields

		/// <summary>
		/// The cache dictionary.
		/// </summary>
		private static readonly Dictionary<string, object> _Cache;

		#endregion

		#region Methods

		/// <summary>
		/// Load a resource from content pipeline.
		/// </summary>
		/// <typeparam name="T">Resource type.</typeparam>
		/// <param name="assetName">Asset name.</param>
		public static void Load<T>(string assetName)
		{
			var asset = GameCore.Content.Load<T>(assetName);
			_Cache[assetName] = asset;
		}

		/// <summary>
		/// Load a specific resource from the cache.
		/// </summary>
		/// <typeparam name="T">Resource type.</typeparam>
		/// <param name="assetName">Asset name</param>
		/// <returns></returns>
		public static T Get<T>(string assetName)
		{
			if(!_Cache.ContainsKey(assetName))
				Load<T>(assetName);

			return (T)_Cache[assetName];
		}

		/// <summary>
		/// Remove an asset from cache.
		/// </summary>
		/// <param name="assetName"></param>
		public static void Remove(string assetName)
		{
			if (!_Cache.ContainsKey(assetName))
				return;

			var obj = _Cache[assetName] as IDisposable;
			if (obj != null)
				obj.Dispose();

			_Cache.Remove(assetName);
		}

		/// <summary>
		/// Remove all entries from the cache.
		/// </summary>
		public static void Clear()
		{
			foreach (var curr in _Cache.Values)
			{
				var obj = curr as IDisposable;
				if(obj != null)
					obj.Dispose();
			}

			_Cache.Clear();
		}

		#endregion
	}
}
