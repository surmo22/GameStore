using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

[BsonIgnoreExtraElements]
public class MongoGame
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ProductID")]
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductKey { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("SupplierID")]
    public int SupplierId { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("CategoryID")]
    public int CategoryId { get; set; }

    public string QuantityPerUnit { get; set; }

    public double UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public int UnitsOnOrder { get; set; }

    public int ReorderLevel { get; set; }

    public bool Discontinued { get; set; }
    
    public int ViewCount { get; set; }
}