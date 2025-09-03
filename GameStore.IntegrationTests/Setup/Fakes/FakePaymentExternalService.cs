using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;

namespace GameStore.IntegrationTests.Setup.Fakes;

public class FakePaymentExternalService : IPaymentExternalService
{
    public Task PayWithVisa(string endPoint, VisaPaymentRequest request)
    {
        return Task.CompletedTask;
    }

    public Task<BoxPaymentResponse> PayWithBox(string endPoint, BoxPaymentRequest request)
    {
        return Task.FromResult(new BoxPaymentResponse
        {
            AccountId = Guid.NewGuid(),
            Amount = 100,
            OrderId = Guid.NewGuid().ToString(),
            PaymentMethod = 1,
            UserId = Guid.NewGuid(),
        });
    }
}