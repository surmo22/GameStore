using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.Domain.MongoEntities.CustomSerializers;
public class IntSerializer : SerializerBase<int>
{
    public override int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;

        switch (bsonReader.CurrentBsonType)
        {
            case BsonType.Int32:
                return bsonReader.ReadInt32();
            default:
                bsonReader.ReadString();
                return 0;
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value)
    {
        context.Writer.WriteInt32(value);
    }
}
