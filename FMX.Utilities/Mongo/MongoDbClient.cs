using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMX.Utilities
{
    /// <summary>
    /// Simple MongoDB utility that will handle inital setup
    /// </summary>
    public class MongoDbClient
    {
        private MongoClient _client;
        private IMongoDatabase _db;

        /// <summary>
        /// Initiates the MongoDB client and database
        /// </summary>
        /// <param name="connectionString">A MongoDB server authentication string.</param>
        /// <param name="databaseName">The name of the MongoDB database that will be used for collections.</param>
        public MongoDbClient(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Retrieves a collection from the db.
        /// </summary>
        /// <typeparam name="T">Type of documents the the collection stores.</typeparam>
        /// <param name="collectionName">Name of the collection in the database.</param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName);
        }
    }
}
