using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;

namespace GameStore.BLL.Services.ExternalServices;

public class AuthentificationExternalService(HttpClient httpClient) : BaseHttpClient(httpClient), IAuthentificationExternalService
{
    public async Task<AuthentificationResponse> Authenticate(AuthentificationRequest request, CancellationToken cancellationToken)
    {
        return await PostAsync<AuthentificationRequest, AuthentificationResponse>(string.Empty, request, cancellationToken);
    }
}