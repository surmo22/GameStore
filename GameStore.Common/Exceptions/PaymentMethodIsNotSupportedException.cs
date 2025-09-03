namespace GameStore.Common.Exceptions;

public class PaymentMethodIsNotSupportedException(string method) : Exception($"Payment method {method} is not supported");
