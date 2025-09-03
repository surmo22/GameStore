using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;
[Route("[controller]")]
[ApiController]
public class ShippersController(IShippersService shippersService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<GetShipperDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await shippersService.GetAllShippersAsync(cancellationToken);
    }
}
