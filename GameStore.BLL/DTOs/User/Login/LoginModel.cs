namespace GameStore.BLL.DTOs.User.Login;

public class LoginModel
{
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public bool InternalAuth { get; set; }
}