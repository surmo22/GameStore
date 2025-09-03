namespace GameStore.BLL.DTOs.User.Creation;

public class CreateUserDto
{
    public UserInfo User { get; set; }
    
    public List<Guid> Roles { get; set; }
    
    public string Password { get; set; }
}