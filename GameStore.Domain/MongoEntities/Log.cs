using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

[BsonIgnoreExtraElements]
public class Log
{
    public DateTime Timestamp { get; set; }
    
    public string Action { get; set; }
    
    public string EntityType { get; set; }
    
    public object? OldVersion { get; set; }
    
    public object? NewVersion { get; set; }
}