using UkChangeCalculator.Exceptions;
using UkChangeCalculator.Services;
using UkChangeCalculator.Utilities;

namespace UkChangeCalculator;

class Program
{
    static void Main()
    {
        var calculator = new ChangeCalculator(new UkDenominationProvider());

        Console.WriteLine("UK Change Calculator");
        Console.WriteLine("====================");

        Console.Write("Enter amount paid:    £");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amountPaid))
        {
            Console.WriteLine("Invalid amount entered.");
            return;
        }

        Console.Write("Enter product price:  £");
        if (!decimal.TryParse(Console.ReadLine(), out decimal productPrice))
        {
            Console.WriteLine("Invalid price entered.");
            return;
        }

        try
        {
            decimal change = InputValidator.GetChange(amountPaid, productPrice);

            if (change == 0)
            {
                Console.WriteLine("\nNo change.");
                return;
            }

            Console.WriteLine("\nYour change is:");
            foreach (var item in calculator.CalculateChange(change))
            {
                Console.WriteLine(item);
            }
        }
        catch (InsufficientPaymentException ex)
        {
            Console.WriteLine("\n" + ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("\n" + ex.Message);
        }
    }
}
