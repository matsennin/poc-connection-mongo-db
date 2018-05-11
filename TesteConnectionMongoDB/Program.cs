using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace TesteConnectionMongoDB
{
    class Program
    {
        private static MongoClient _mongoConnection;
        private static IMongoDatabase _db;
        public static string _rand;
        public static void Initialize()
        {
            _mongoConnection = CreateConnection();
            _db = UseCreateDatabase(_mongoConnection);
            _rand = $"Matheus_{Guid.NewGuid().ToString()}";
        }
        
        static void Main(string[] args)
        {
            try
            {
                Initialize();                
                VerifyConnectionDatabase(_mongoConnection);
                Select(_db);
                Incluir(_db, _rand);
                Update(_db, _rand, $"{_rand}_1");
                Delete(_db, $"{_rand}_1");
                //Delete2(_db, $"{_rand}_1");
                VerifyConnectionDatabase(_mongoConnection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            Console.ReadLine();
        }

        private static MongoClient CreateConnection()
        {
            //Create Connection 
            var mongoDBConnection = ConfigurationManager.AppSettings["MongoDBConnection@gmail"].ToString().Replace("##", "&");
            return new MongoClient(mongoDBConnection);
        }

        private static IMongoDatabase UseCreateDatabase(MongoClient client, string databaseName = "DatabaseTeste")
        {
            // Creating/Getting a mongo Database
            return client.GetDatabase(databaseName);
        }

        private static bool VerifyConnectionDatabase(MongoClient client)
        {
            //Verifying mongo connection state
            foreach (var x in client.Cluster.Description.Servers)
            {
                Console.WriteLine(x.State);
                if (x.State == MongoDB.Driver.Core.Servers.ServerState.Connected)
                    return true;
            }
            return false;
        }

        private static void Select(IMongoDatabase db)
        {
            // Creating/Getting a mongo Collection[Table]
            var collection = db.GetCollection<BsonDocument>("myCollection");
            using (var cursor = collection.Find(new BsonDocument()).ToCursorAsync())
            {
                while (cursor.Result.MoveNext())
                {
                    foreach (var doc in cursor.Result.Current)
                    {
                        Console.WriteLine(doc);
                    }
                }
            }
        }

        private static void Incluir(IMongoDatabase db, string rand)
        {
            // Inserting a line in a mongo Collection[Table]
            var collectionInsert = db.GetCollection<BsonDocument>("myCollection");
            var campos = new Dictionary<string, object>();
            campos.Add("Nome", rand);
            campos.Add("CPF", "422339148802");
            campos.Add("RG", "4426001122");
            campos.Add("DataNascimento", DateTime.Now.ToString());
            collectionInsert.InsertOne(new BsonDocument(campos));
            Console.WriteLine("\nInsert");
        }

        private static void Update(IMongoDatabase db, string filtervalue, string newvalue)
        {
            //Updating mongo line Collection[Table]
            var collectionUpdate = db.GetCollection<BsonDocument>("myCollection");
            var filter = Builders<BsonDocument>.Filter.Eq("Nome", filtervalue);
            var update = Builders<BsonDocument>.Update
                .Set("Nome", newvalue)
                .CurrentDate("lastModified");
            var updateResult = collectionUpdate.UpdateOne(filter, update);
            Console.WriteLine($"\nUpdate: {updateResult.ModifiedCount}");
        }

        private static void Delete(IMongoDatabase db, string rand)
        {
            //Deleting mongo line Collection[Table]
            var collectionDelete = db.GetCollection<BsonDocument>("myCollection");
            var filterDelete = Builders<BsonDocument>.Filter.Eq("Nome", rand);
            var deleteResult = collectionDelete.DeleteOne(filterDelete);
            Console.WriteLine($"\nDelete: {deleteResult.DeletedCount.ToString()}");
        }

        //private static void Delete2(IMongoDatabase db, string rand)
        //{
        //    //Deleting mongo line Collection[Table]

        //    var opcoes = new MongoCollectionSettings
        //    {
        //        WriteConcern = WriteConcern.Acknowledged
        //    };

        //    var collectionDelete = db.GetCollection("myCollection", opcoes);
        //    var filterDelete = Builders<myCollection>.Filter.Eq(x => x.Nome, rand);

        //    //var deleteResult = collectionDelete.Find(filterDelete);
        //    //Console.WriteLine($"\nDelete: {deleteResult.DeletedCount.ToString()}");
        //}
    }

    [BsonSerializer]
    public class myCollection : MongoBase
    {
        [BsonElement("Nome")]
        public string Nome { get; set; }

        [BsonElement("CPF")]
        public string CPF { get; set; }

        [BsonElement("RG")]
        public string RG { get; set; }

        [BsonElement("DataNascimento")]
        public string DataNascimento { get; set; }

    }
}
