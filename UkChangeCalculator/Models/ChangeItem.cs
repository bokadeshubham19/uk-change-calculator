namespace UkChangeCalculator.Models;

// One line of the change breakdown, e.g. 2 x £2.
public sealed record ChangeItem(Denomination Denomination, int Quantity)
{
    public override string ToString() => $"{Quantity} x {Denomination.Name}";
}
