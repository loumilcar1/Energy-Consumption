using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyConsumption
{
    public class MongoDBHandler
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBHandler(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task ClearCollectionAsync()
        {
            await _collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
        }

        public async Task SaveCommunityDataAsync(string community, List<(double Value, DateTime Datetime)> data)
        {
            var document = new BsonDocument
            {
                { "community", community },
                { "values", new BsonArray(data.ConvertAll(d =>
                    new BsonDocument
                    {
                        { "value", d.Value },
                        { "datetime", d.Datetime }
                    }
                ))}
            };

            await _collection.InsertOneAsync(document);
        }
    }
}
