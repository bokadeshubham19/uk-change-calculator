namespace UkChangeCalculator.Models;

// A note or coin. We keep the value in pence (an int) so all the change
// maths can be done with integers instead of decimals/doubles.
public sealed class Denomination
{
    public Denomination(string name, int valueInPence)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (valueInPence <= 0)
            throw new ArgumentOutOfRangeException(nameof(valueInPence), "Value must be greater than zero.");

        Name = name;
        ValueInPence = valueInPence;
    }

    // e.g. "£10" or "50p"
    public string Name { get; }

    public int ValueInPence { get; }
}
