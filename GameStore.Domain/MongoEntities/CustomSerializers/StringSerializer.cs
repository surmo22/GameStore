using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.Domain.MongoEntities.CustomSerializers;

public class StringSerializer : SerializerBase<string>
{
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;

        return bsonReader.CurrentBsonType switch
        {
            BsonType.Int32 => bsonReader.ReadInt32().ToString(),
            _ => bsonReader.ReadString()
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        context.Writer.WriteString(value);
    }
}