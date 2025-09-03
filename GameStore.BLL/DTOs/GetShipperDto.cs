namespace GameStore.BLL.DTOs;

public class GetShipperDto
{
    public string Id { get; set; }

    public string ShipperId { get; set; }

    public Dictionary<string, object> AdditionalFields { get; set; }
}