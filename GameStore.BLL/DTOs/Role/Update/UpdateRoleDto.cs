using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.DTOs.Role.Update;

public class UpdateRoleDto
{
    public UpdateRoleInfo Role { get; set; }
    
    public List<UserPermissionTypes> Permissions { get; set; }
}