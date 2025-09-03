using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.DTOs.Role.Create;

public class CreateRoleDto
{
    public CreateRoleInfo Role { get; set; }
    
    public List<UserPermissionTypes> Permissions { get; set; }
}