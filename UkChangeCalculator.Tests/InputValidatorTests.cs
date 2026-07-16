using NUnit.Framework;
using UkChangeCalculator.Exceptions;
using UkChangeCalculator.Utilities;

namespace UkChangeCalculator.Tests;

[TestFixture]
public class InputValidatorTests
{
    [Test]
    public void Returns_change_for_a_normal_payment()
    {
        Assert.That(InputValidator.GetChange(20m, 5.50m), Is.EqualTo(14.50m));
    }

    [Test]
    public void Exact_payment_gives_zero_change()
    {
        Assert.That(InputValidator.GetChange(9.99m, 9.99m), Is.EqualTo(0m));
    }

    [Test]
    public void Paying_too_little_throws()
    {
        var ex = Assert.Throws<InsufficientPaymentException>(() => InputValidator.GetChange(5m, 5.50m));

        Assert.That(ex!.AmountPaid, Is.EqualTo(5m));
        Assert.That(ex.ProductPrice, Is.EqualTo(5.50m));
    }

    [TestCase(-1)]
    [TestCase(-0.01)]
    public void Negative_amount_paid_throws(decimal amountPaid)
    {
        Assert.That(() => InputValidator.GetChange(amountPaid, 1m), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Negative_price_throws()
    {
        Assert.That(() => InputValidator.GetChange(10m, -1m), Throws.TypeOf<ArgumentException>());
    }

    [TestCase(5.555)]
    [TestCase(0.001)]
    public void More_than_two_decimal_places_throws(decimal amount)
    {
        Assert.That(() => InputValidator.ValidateAmount(amount, "Amount"), Throws.TypeOf<ArgumentException>());
    }

    [TestCase(0)]
    [TestCase(5.5)]
    [TestCase(12.34)]
    [TestCase(1000000.99)]
    public void Valid_amounts_pass(decimal amount)
    {
        Assert.That(() => InputValidator.ValidateAmount(amount, "Amount"), Throws.Nothing);
    }
}
