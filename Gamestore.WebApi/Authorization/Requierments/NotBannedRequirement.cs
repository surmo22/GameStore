using Microsoft.AspNetCore.Authorization;

namespace GameStore.WebApi.Authorization.Requierments;

public class NotBannedRequirement : IAuthorizationRequirement
{
}