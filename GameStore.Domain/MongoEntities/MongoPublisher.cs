using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

[BsonIgnoreExtraElements]
public class MongoPublisher
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("SupplierID")]
    public int SupplierId { get; set; }

    public string CompanyName { get; set; }

    public string ContactName { get; set; }

    public string ContactTitle { get; set; }

    [BsonIgnore]
    public string Address => AddressRaw?.ToString() ?? string.Empty;

    [BsonElement("Address")]
    public BsonValue? AddressRaw { get; set; }

    [BsonIgnore]
    public string City => CityRaw?.ToString() ?? string.Empty;

    [BsonElement("City")]
    public BsonValue? CityRaw { get; set; }

    [BsonIgnore]
    public string Region => RegionRaw?.ToString() ?? string.Empty;

    [BsonElement("Region")]
    public BsonValue? RegionRaw { get; set; }

    [BsonIgnore]
    public string PostalCode => PostalCodeRaw?.ToString() ?? string.Empty;

    [BsonElement("PostalCode")]
    public BsonValue? PostalCodeRaw { get; set; }

    [BsonIgnore]
    public string Country => CountryRaw?.ToString() ?? string.Empty;

    [BsonElement("Country")]
    public BsonValue? CountryRaw { get; set; }

    [BsonIgnore]
    public string Phone => PhoneRaw?.ToString() ?? string.Empty;

    [BsonElement("Phone")]
    public BsonValue? PhoneRaw { get; set; }

    [BsonIgnore]
    public string Fax => FaxRaw?.ToString() ?? string.Empty;

    [BsonElement("Fax")]
    public BsonValue? FaxRaw { get; set; }

    [BsonIgnore]
    public string HomePage => HomePageRaw?.ToString() ?? string.Empty;

    [BsonElement("HomePage")]
    public BsonValue? HomePageRaw { get; set; }
}