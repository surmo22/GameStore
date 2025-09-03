namespace GameStore.Common.Exceptions;

public class PaymentFailedException(string message, Exception ex) : Exception(message, ex);
