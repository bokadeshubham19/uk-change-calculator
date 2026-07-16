using UkChangeCalculator.Models;

namespace UkChangeCalculator.Services;

/// <summary>
/// Greedy change calculator. For each denomination (largest first) it takes as
/// many as fit, then moves on with the remainder. The UK coin set is canonical,
/// so this always gives the smallest number of pieces.
/// </summary>
public sealed class ChangeCalculator : IChangeCalculator
{
    private readonly IDenominationProvider _denominations;

    public ChangeCalculator(IDenominationProvider denominations)
    {
        _denominations = denominations ?? throw new ArgumentNullException(nameof(denominations));
    }

    public IReadOnlyList<ChangeItem> CalculateChange(decimal changeAmount)
    {
        if (changeAmount < 0m)
            throw new ArgumentOutOfRangeException(nameof(changeAmount), "Change cannot be negative.");

        // Convert pounds to whole pence and work in integers from here on.
        long remaining = (long)decimal.Round(changeAmount * 100m, 0, MidpointRounding.AwayFromZero);

        var result = new List<ChangeItem>();

        foreach (var denomination in _denominations.GetDenominations())
        {
            if (remaining < denomination.ValueInPence)
                continue;

            int count = (int)(remaining / denomination.ValueInPence);
            remaining %= denomination.ValueInPence;

            result.Add(new ChangeItem(denomination, count));

            if (remaining == 0)
                break;
        }

        return result;
    }
}
