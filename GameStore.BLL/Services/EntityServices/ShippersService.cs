using AutoMapper;
using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;

namespace GameStore.BLL.Services.EntityServices;

public class ShippersService(IShippersRepository repository, IMapper mapper) : IShippersService
{
    public async Task<IEnumerable<GetShipperDto>> GetAllShippersAsync(CancellationToken cancellationToken)
    {
        var shippers = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<GetShipperDto>>(shippers);
    }
}