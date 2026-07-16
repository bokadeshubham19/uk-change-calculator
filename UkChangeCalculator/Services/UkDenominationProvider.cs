using UkChangeCalculator.Models;

namespace UkChangeCalculator.Services;

/// <summary>
/// The standard UK notes and coins, largest first.
/// </summary>
public sealed class UkDenominationProvider : IDenominationProvider
{
    private static readonly IReadOnlyList<Denomination> _denominations = new List<Denomination>
    {
        new("£50", 5000),
        new("£20", 2000),
        new("£10", 1000),
        new("£5", 500),
        new("£2", 200),
        new("£1", 100),
        new("50p", 50),
        new("20p", 20),
        new("10p", 10),
        new("5p", 5),
        new("2p", 2),
        new("1p", 1),
    };

    public IReadOnlyList<Denomination> GetDenominations() => _denominations;
}
