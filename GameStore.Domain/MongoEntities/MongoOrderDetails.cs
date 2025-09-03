using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoOrderDetails
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("OrderID")]
    public int OrderId { get; set; }
    
    [BsonElement("ProductID")]
    public int ProductId { get; set; }
    
    [BsonElement("UnitPrice")]
    public decimal UnitPrice { get; set; }
    
    [BsonElement("Quantity")]
    public int Quantity { get; set; }
    
    [BsonElement("Discount")]
    public decimal Discount { get; set; }
}