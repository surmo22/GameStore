namespace GameStore.BLL.Options;

public class FilePathOptions
{
    public string BaseDirectory { get; init; } = string.Empty;

    public string TimeStampPattern { get; init; } = "dd_MM_yyyy";

    public string BaseFolder { get; init; } = "games";
}