using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;

namespace GameStore.IntegrationTests.Setup.Fakes;

public class FakeExternalAuthService : IAuthentificationExternalService
{
    public Task<AuthentificationResponse> Authenticate(AuthentificationRequest request, CancellationToken cancellationToken)
    {
        var response = new AuthentificationResponse
        {
            LastName = "test",
            Email = "test@test.com"
        };
        return Task.FromResult(response);
    }
}