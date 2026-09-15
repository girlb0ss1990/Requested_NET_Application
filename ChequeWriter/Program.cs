using System.Globalization;
using ChequeWriter;

Console.Write("Enter a cheque amount: ");
string? input = Console.ReadLine();

if (!decimal.TryParse(
        input,
        NumberStyles.Number,
        CultureInfo.InvariantCulture,
        out decimal amount))
{
    Console.Error.WriteLine(
        "Invalid amount. Enter a value such as 1234.56.");

    Environment.ExitCode = 1;
    WaitBeforeExit();
    return;
}

try
{
    string words = ChequeAmountConverter.Convert(amount);
    Console.WriteLine(words);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}

WaitBeforeExit();

static void WaitBeforeExit()
{
    // Keeps the window open when the .exe is double-clicked.
    if (!Console.IsInputRedirected)
    {
        Console.WriteLine();
        Console.Write("Press Enter to close...");
        Console.ReadLine();
    }
}
