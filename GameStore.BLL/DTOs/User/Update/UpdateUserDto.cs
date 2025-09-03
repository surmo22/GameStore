namespace GameStore.BLL.DTOs.User.Update;

public class UpdateUserDto
{
    public UpdateUserInfo User { get; set; }
    
    public List<Guid> Roles { get; set; }
    
    public string Password { get; set; }
}