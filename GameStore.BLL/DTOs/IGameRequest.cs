namespace GameStore.BLL.DTOs;

public interface IGameRequest
{
    List<Guid> Genres { get; }

    List<Guid> Platforms { get; }

    Guid PublisherId { get; }
    
    public string Image { get; }
}