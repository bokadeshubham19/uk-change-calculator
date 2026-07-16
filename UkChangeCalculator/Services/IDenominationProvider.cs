using UkChangeCalculator.Models;

namespace UkChangeCalculator.Services;

/// <summary>
/// Supplies the denominations the calculator is allowed to use.
/// Swapping the implementation lets us support a different currency
/// (or a till with a limited float) without touching the calculator.
/// </summary>
public interface IDenominationProvider
{
    // Ordered largest first.
    IReadOnlyList<Denomination> GetDenominations();
}
