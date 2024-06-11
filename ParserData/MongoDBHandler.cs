using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserData
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

        public async Task<List<DataCSV>> GetDataAsync()
        {
            var documents = await _collection.Find(new BsonDocument()).ToListAsync();
            var records = new List<DataCSV>();

            foreach (var document in documents)
            {
                var id = document["_id"].ToString();
                var values = document["values"].AsBsonArray;

                foreach (var valueDoc in values)
                {
                    var value = valueDoc["value"].ToDouble();
                    var datetime = valueDoc["datetime"].ToUniversalTime().ToString("o");

                    var record = new DataCSV { Id = id, Value = value, DateTime = datetime };
                    records.Add(record);
                }
            }

            return records;
        }
    }
    public class DataCSV
    {
        public string Id { get; set; }
        public double Value { get; set; }
        public string DateTime { get; set; }
    }
}
