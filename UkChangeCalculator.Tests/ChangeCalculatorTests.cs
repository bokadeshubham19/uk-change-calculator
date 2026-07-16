using NUnit.Framework;
using UkChangeCalculator.Models;
using UkChangeCalculator.Services;

namespace UkChangeCalculator.Tests;

[TestFixture]
public class ChangeCalculatorTests
{
    private ChangeCalculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        _calculator = new ChangeCalculator(new UkDenominationProvider());
    }

    // value in pence -> quantity, handy for asserting a whole breakdown
    private static Dictionary<int, int> ToMap(IReadOnlyList<ChangeItem> items) =>
        items.ToDictionary(i => i.Denomination.ValueInPence, i => i.Quantity);

    [Test]
    public void Returns_expected_breakdown_for_the_example()
    {
        // £20 paid on a £5.50 item = £14.50 change
        var result = _calculator.CalculateChange(14.50m);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].Denomination.Name, Is.EqualTo("£10"));
        Assert.That(result[0].Quantity, Is.EqualTo(1));
        Assert.That(result[1].Denomination.Name, Is.EqualTo("£2"));
        Assert.That(result[1].Quantity, Is.EqualTo(2));
        Assert.That(result[2].Denomination.Name, Is.EqualTo("50p"));
        Assert.That(result[2].Quantity, Is.EqualTo(1));
    }

    [Test]
    public void Returns_empty_breakdown_for_zero()
    {
        Assert.That(_calculator.CalculateChange(0m), Is.Empty);
    }

    [Test]
    public void Breakdown_is_always_largest_first()
    {
        var values = _calculator.CalculateChange(88.88m)
            .Select(i => i.Denomination.ValueInPence)
            .ToList();

        Assert.That(values, Is.Ordered.Descending);
    }

    [TestCase(0.01, 1)]
    [TestCase(0.02, 2)]
    [TestCase(0.05, 5)]
    public void Handles_the_smallest_coins(decimal change, int expectedPence)
    {
        var result = _calculator.CalculateChange(change);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Denomination.ValueInPence, Is.EqualTo(expectedPence));
        Assert.That(result[0].Quantity, Is.EqualTo(1));
    }

    [Test]
    public void Handles_awkward_decimal_amounts()
    {
        // £3.99 = £2 + £1 + 50p + 2x20p + 5p + 2x2p
        var map = ToMap(_calculator.CalculateChange(3.99m));

        Assert.That(map[200], Is.EqualTo(1));
        Assert.That(map[100], Is.EqualTo(1));
        Assert.That(map[50], Is.EqualTo(1));
        Assert.That(map[20], Is.EqualTo(2));
        Assert.That(map[5], Is.EqualTo(1));
        Assert.That(map[2], Is.EqualTo(2));
        Assert.That(map.ContainsKey(1), Is.False);
    }

    [Test]
    public void Handles_large_amounts()
    {
        // £1,234.56
        var map = ToMap(_calculator.CalculateChange(1234.56m));

        Assert.That(map[5000], Is.EqualTo(24)); // 24 x £50 = £1200
        Assert.That(map[2000], Is.EqualTo(1));
        Assert.That(map[1000], Is.EqualTo(1));
        Assert.That(map[200], Is.EqualTo(2));
        Assert.That(map[50], Is.EqualTo(1));
        Assert.That(map[5], Is.EqualTo(1));
        Assert.That(map[1], Is.EqualTo(1));
    }

    [Test]
    public void Exact_note_returns_a_single_note()
    {
        var result = _calculator.CalculateChange(50m);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Denomination.Name, Is.EqualTo("£50"));
        Assert.That(result[0].Quantity, Is.EqualTo(1));
    }

    [Test]
    public void Uses_the_fewest_possible_pieces()
    {
        // £14.50 is 1x£10 + 2x£2 + 1x50p = 4 pieces
        int pieces = _calculator.CalculateChange(14.50m).Sum(i => i.Quantity);

        Assert.That(pieces, Is.EqualTo(4));
    }

    [TestCase(0.014, 1)]
    [TestCase(0.015, 2)]
    public void Rounds_sub_penny_amounts_to_the_nearest_penny(decimal change, int expectedPence)
    {
        var result = _calculator.CalculateChange(change);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Denomination.ValueInPence, Is.EqualTo(expectedPence));
    }

    [Test]
    public void Negative_change_throws()
    {
        Assert.That(() => _calculator.CalculateChange(-1m), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Null_provider_throws()
    {
        Assert.That(() => new ChangeCalculator(null!), Throws.TypeOf<ArgumentNullException>());
    }
}
