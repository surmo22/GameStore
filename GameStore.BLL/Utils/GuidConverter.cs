namespace GameStore.BLL.Utils;

public static class GuidConverter
{
    public static Guid? ConvertStringToGuid(string? id)
    {
        try
        {
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}