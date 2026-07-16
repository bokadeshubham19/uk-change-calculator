namespace UkChangeCalculator.Exceptions;

// Raised when the customer hasn't paid enough to cover the price.
public sealed class InsufficientPaymentException : Exception
{
    public InsufficientPaymentException(decimal amountPaid, decimal productPrice)
        : base($"Amount paid (£{amountPaid:0.00}) is less than the price (£{productPrice:0.00}). " +
               $"Short by £{productPrice - amountPaid:0.00}.")
    {
        AmountPaid = amountPaid;
        ProductPrice = productPrice;
    }

    public decimal AmountPaid { get; }

    public decimal ProductPrice { get; }
}
