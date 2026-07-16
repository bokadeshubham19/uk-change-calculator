using UkChangeCalculator.Models;

namespace UkChangeCalculator.Services;

/// <summary>
/// Works out which notes and coins make up a given amount of change.
/// </summary>
public interface IChangeCalculator
{
    /// <summary>
    /// Breaks <paramref name="changeAmount"/> (in pounds) into the fewest notes
    /// and coins, largest first. Returns an empty list when the amount is zero.
    /// </summary>
    IReadOnlyList<ChangeItem> CalculateChange(decimal changeAmount);
}
