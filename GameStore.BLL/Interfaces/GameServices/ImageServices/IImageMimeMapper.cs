namespace GameStore.BLL.Interfaces.GameServices.ImageServices;

public interface IImageMimeMapper
{
    string GetExtension(string mimeType);
}