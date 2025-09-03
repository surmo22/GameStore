using GameStore.Domain.MongoEntities;
using MongoDB.Driver;

namespace GameStore.Data.Data;

public class NorthwindMongoContext
{
#pragma warning disable IDE0290 // Suppress primary constructor warning
    public NorthwindMongoContext(IMongoClient client)
    {
        Database = client.GetDatabase("Northwind");
    }
#pragma warning restore

    public IMongoCollection<MongoShipper> Shippers => Database.GetCollection<MongoShipper>("shippers");

    public IMongoCollection<MongoGame> Games => Database.GetCollection<MongoGame>("products");

    public IMongoCollection<MongoGenre> Genres => Database.GetCollection<MongoGenre>("categories");

    public IMongoCollection<MongoPublisher> Publishers => Database.GetCollection<MongoPublisher>("suppliers");
    
    public IMongoCollection<MongoOrder> Orders => Database.GetCollection<MongoOrder>("orders");
    
    public IMongoCollection<MongoOrderDetails> OrderDetails => Database.GetCollection<MongoOrderDetails>("order-details");
    
    public IMongoDatabase Database { get; }
}