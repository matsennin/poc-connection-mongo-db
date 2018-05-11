using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace TesteConnectionMongoDB
{
    /// <summary>
    /// Define atributos comuns de classes de persistências MongoDB.
    /// </summary>
    public abstract class MongoBase
    {
        /// <summary>
        /// ID único do objeto (ObjectId).
        /// </summary>
        [BsonId(IdGenerator = typeof(BsonObjectIdGenerator))]
        public BsonObjectId ID { get; set; }

        /// <summary>
        /// ID único do objeto como string
        /// </summary>
        [BsonIgnore()]
        public string StringID
        {
            get
            {
                return ID != null ? ID.ToString() : null;
            }
            set
            {
                if (value != null)
                    ID = new BsonObjectId(new ObjectId(value));
            }
        }

        /// <summary>
        /// Contém quaisquer valores que esteja no BSON e não há propriedade definida para si.
        /// </summary>
        [BsonExtraElements]
        public BsonDocument OutrosAtributos { get; set; }
    }
}
