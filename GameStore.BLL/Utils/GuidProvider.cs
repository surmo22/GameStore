using GameStore.BLL.Interfaces;

namespace GameStore.BLL.Utils;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}