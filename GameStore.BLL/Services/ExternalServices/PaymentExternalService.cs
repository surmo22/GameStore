using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;

namespace GameStore.BLL.Services.ExternalServices;

public class PaymentExternalService(HttpClient httpClient) : BaseHttpClient(httpClient), IPaymentExternalService
{
    public async Task PayWithVisa(string endPoint, VisaPaymentRequest request)
    {
        await PostAsync<VisaPaymentRequest, VisaResult>(endPoint, request, CancellationToken.None);
    }

    public async Task<BoxPaymentResponse> PayWithBox(string endPoint, BoxPaymentRequest request)
    {
        return await PostAsync<BoxPaymentRequest, BoxPaymentResponse>(endPoint, request, CancellationToken.None);
    }
}