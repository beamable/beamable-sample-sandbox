using System.Collections.Generic;
using Beamable.Common;
using Beamable.Common.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("ServiceDataStorage")]
	public class ServiceDataStorage : MongoStorageObject
	{
	}

	public static class ServiceDataStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for ServiceDataStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> ServiceDataStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<ServiceDataStorage>();

		/// <summary>
		/// Gets a MongoDB collection from ServiceDataStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="ServiceDataStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ServiceDataStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<ServiceDataStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from ServiceDataStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="ServiceDataStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ServiceDataStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<ServiceDataStorage, TCollection>();
		
		/// <summary>
		/// Updates the lobby data with the given ID using the provided data.
		/// </summary>
		public static async Promise<bool> Update<T>(this IStorageObjectConnectionProvider provider, string id, T updatedData) 
			where T : StorageDocument, ISetStorageDocument<T>
		{
			var collection = await provider.GetCollection<ServiceDataStorage, T>();
			var documentToUpdate = await provider.GetById<T>(id);
			if (documentToUpdate == null)
				return false;

			documentToUpdate.Set(updatedData);
			var result = await collection.ReplaceOneAsync(provider.GetFilterById<T>(id), documentToUpdate);
			return result.ModifiedCount > 0;
		}

		/// <summary>
		/// Gets all lobby data objects from the database.
		/// </summary>
		public static async Promise<List<T>> GetAll<T>(this IStorageObjectConnectionProvider provider)
			where T : StorageDocument
		{
			var collection = await provider.GetCollection<ServiceDataStorage, T>();
			return collection.Find(data => true).ToList();
		}

		/// <summary>
		/// Gets a lobby data object by its ID.
		/// </summary>
		public static async Promise<T> GetById<T>(this IStorageObjectConnectionProvider provider, string id)
			where T : StorageDocument
		{
			var collection = await provider.GetCollection<ServiceDataStorage, T>();
			var search = await collection.FindAsync(provider.GetFilterById<T>(id));
			return search.FirstOrDefault();
		}
		
		/// <summary>
		/// Gets a MongoDB ID filter for a given <see cref="StorageDocument"/>
		/// </summary>
		/// <typeparam name="T">A <see cref="StorageDocument"/> derived type you want to filter.</typeparam>
		private static FilterDefinition<T> GetFilterById<T>(this IStorageObjectConnectionProvider provider, string id)
			where T : StorageDocument
			=> Builders<T>.Filter.Eq("_id", new ObjectId(id));
		
		/// <summary>
		/// Gets an object of given type by field name.
		/// </summary>
		public static async Promise<T> GetByFieldName<T, TValue>(this IStorageObjectConnectionProvider provider,
			string field, TValue value) where T : StorageDocument
		{
			var collection = await provider.GetCollection<ServiceDataStorage, T>();
			var search = await collection.FindAsync(provider.GetFilterByField<T, TValue>(field, value));
			return search.FirstOrDefault();
		}
		
		/// <summary>
		/// Gets a MongoDB field filter for a given <see cref="StorageDocument"/>
		/// </summary>
		/// <typeparam name="T">A <see cref="StorageDocument"/> derived type you want to filter.</typeparam>
		/// <typeparam name="TValue"></typeparam>
		private static FilterDefinition<T> GetFilterByField<T, TValue>(this IStorageObjectConnectionProvider provider,
			string field, TValue value) where T : StorageDocument
			=> Builders<T>.Filter.Eq(field, value);
	}
}