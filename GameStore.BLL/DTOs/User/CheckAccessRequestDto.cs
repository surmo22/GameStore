namespace GameStore.BLL.DTOs.User;

public class CheckAccessRequestDto
{
    public string TargetPage { get; set; }
    
    public string? TargetId { get; set; }
}