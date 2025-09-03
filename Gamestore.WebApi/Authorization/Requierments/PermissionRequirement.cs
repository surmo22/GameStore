using Microsoft.AspNetCore.Authorization;

namespace GameStore.WebApi.Authorization.Requierments;

public record PermissionRequirement(string Permission) : IAuthorizationRequirement
{
}