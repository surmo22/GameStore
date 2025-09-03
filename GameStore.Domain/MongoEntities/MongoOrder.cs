using GameStore.Domain.MongoEntities.CustomSerializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

[BsonIgnoreExtraElements]
public class MongoOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("OrderID")]
    [BsonSerializer(typeof(IntSerializer))]
    public int OrderId { get; set; }
    
    [BsonElement("CustomerID")]
    [BsonSerializer(typeof(IntSerializer))]
    public int CustomerId { get; set; }
    
    [BsonElement("EmployeeID")]
    [BsonSerializer(typeof(IntSerializer))]
    public int EmployeeId { get; set; }
    
    [BsonElement("OrderDate")]
    [BsonSerializer(typeof(DateTimeSerializer))]
    public DateTime OrderDate { get; set; }
    
    [BsonElement("RequiredDate")]
    [BsonSerializer(typeof(DateTimeSerializer))]
    public DateTime RequiredDate { get; set; }
    
    [BsonElement("ShippedDate")]
    [BsonSerializer(typeof(DateTimeSerializer))]
    public DateTime ShippedDate { get; set; }
    
    [BsonElement("ShipVia")]
    [BsonSerializer(typeof(IntSerializer))]
    public int ShipVia { get; set; }
    
    [BsonElement("ShipName")]
    public string ShipName { get; set; }
    
    [BsonElement("Freight")]
    public decimal Freight { get; set; }
    
    [BsonElement("ShipAddress")]
    [BsonSerializer(typeof(StringSerializer))]
    public string ShipAddress { get; set; }
    
    [BsonElement("ShipCity")]
    [BsonSerializer(typeof(StringSerializer))]
    public string ShipCity { get; set; }
    
    [BsonElement("ShipRegion")]
    [BsonSerializer(typeof(StringSerializer))]
    public string ShipRegion { get; set; }
    
    [BsonElement("ShipPostalCode")]
    [BsonSerializer(typeof(StringSerializer))]
    public string ShipPostalCode { get; set; }
    
    [BsonElement("ShipCountry")]
    [BsonSerializer(typeof(StringSerializer))]
    public string ShipCountry { get; set; }
}