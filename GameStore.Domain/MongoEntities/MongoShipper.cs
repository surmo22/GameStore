using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoShipper
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ShipperID")]
    public int ShipperId { get; set; }

    [BsonExtraElements]
    public Dictionary<string, object> AdditionalFields { get; set; }
}