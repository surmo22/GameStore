using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.Domain.MongoEntities.CustomSerializers;

public class DateTimeSerializer : SerializerBase<DateTime>
{
    public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;

        switch (bsonReader.CurrentBsonType)
        {
            case BsonType.DateTime:
                return new DateTime(bsonReader.ReadDateTime());
            default:
            {
                var strValue = bsonReader.ReadString();
                return DateTime.TryParse(strValue, out var parsedDate) ? parsedDate :
                    DateTime.MinValue;
            }
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
    {
        context.Writer.WriteDateTime(value.Ticks);
    }
}