using System;
using System.IO;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;

namespace geek.GameEngine
{
	/// <summary>
	/// The storage class that can keep objects in an isolated file storage.
	/// </summary>
	public static class Storage
	{
		/// <summary>
		/// Storage.
		/// </summary>
		private static readonly IsolatedStorageFile _StorageArea = IsolatedStorageFile.GetUserStoreForApplication();

		/// <summary>
		/// Load an object from file.
		/// </summary>
		/// <typeparam name="T">Desired object's type.</typeparam>
		/// <param name="filename">File name.</param>
		/// <param name="knownTypes">Known types.</param>
		/// <returns></returns>
		public static T Load<T>(string filename, params Type[] knownTypes) where T : class
		{
			var serializer = new DataContractSerializer(typeof(T), knownTypes);

			if (!_StorageArea.FileExists(filename))
				throw new ArgumentException(string.Format("Could not find file {0}!", filename));

			using (var stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, _StorageArea))
				return (T)serializer.ReadObject(stream);
		}

		/// <summary>
		/// Serialize an object and store it in a file.
		/// </summary>
		/// <typeparam name="T">Object's type.</typeparam>
		/// <param name="obj">Object.</param>
		/// <param name="filename">File name.</param>
		/// <param name="knownTypes">Known types.</param>
		/// <returns></returns>
		public static bool Save<T>(T obj, string filename, params Type[] knownTypes)
		{
			var serializer = new DataContractSerializer(typeof(T), knownTypes);
			try
			{
				using (var stream = new IsolatedStorageFileStream(filename, FileMode.Create, _StorageArea))
					serializer.WriteObject(stream, obj);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
