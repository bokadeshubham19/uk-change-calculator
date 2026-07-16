using UkChangeCalculator.Exceptions;

namespace UkChangeCalculator.Utilities;

// Keeps the input checks in one place, away from the change-making logic.
public static class InputValidator
{
    public static void ValidateAmount(decimal amount, string label)
    {
        if (amount < 0m)
            throw new ArgumentException($"{label} cannot be negative.", nameof(amount));

        // Anything finer than a penny isn't real money.
        if (decimal.Round(amount, 2) != amount)
            throw new ArgumentException($"{label} cannot have more than two decimal places.", nameof(amount));
    }

    // Validates both figures and returns the change owed.
    public static decimal GetChange(decimal amountPaid, decimal productPrice)
    {
        ValidateAmount(amountPaid, "Amount paid");
        ValidateAmount(productPrice, "Product price");

        if (amountPaid < productPrice)
            throw new InsufficientPaymentException(amountPaid, productPrice);

        return amountPaid - productPrice;
    }
}
