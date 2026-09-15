using System.Globalization;

namespace ChequeWriter;

/// <summary>
/// Converts a cheque amount into New Zealand/British wording.
/// </summary>
public static class ChequeAmountConverter
{
    public const decimal MaxAmount = 999_999_999_999_999.99m;

    private static readonly string[] Ones =
    {
        "zero",
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten",
        "eleven",
        "twelve",
        "thirteen",
        "fourteen",
        "fifteen",
        "sixteen",
        "seventeen",
        "eighteen",
        "nineteen"
    };

    private static readonly string[] Tens =
    {
        "",
        "",
        "twenty",
        "thirty",
        "forty",
        "fifty",
        "sixty",
        "seventy",
        "eighty",
        "ninety"
    };

    private static readonly (long Value, string Name)[] Scales =
    {
        (1_000_000_000_000, "trillion"),
        (1_000_000_000, "billion"),
        (1_000_000, "million"),
        (1_000, "thousand"),
        (1, "")
    };

    public static string Convert(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Cheque amounts cannot be negative.");
        }

        decimal roundedAmount = decimal.Round(
            amount,
            2,
            MidpointRounding.AwayFromZero);

        if (roundedAmount > MaxAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                $"The maximum supported amount is {MaxAmount:N2}.");
        }

        long dollars = decimal.ToInt64(decimal.Truncate(roundedAmount));
        int cents = decimal.ToInt32((roundedAmount - dollars) * 100);

        string dollarWords = ConvertInteger(dollars);
        string centWords = ConvertInteger(cents);

        string dollarUnit = dollars == 1 ? "dollar" : "dollars";
        string centUnit = cents == 1 ? "cent" : "cents";

        string result = $"{dollarWords} {dollarUnit} and {centWords} {centUnit}";
        return CapitaliseFirstLetter(result);
    }

    public static string ConvertInteger(long number)
    {
        if (number == 0)
        {
            return Ones[0];
        }

        var groups = new List<(long Value, string Words)>();
        long remaining = number;

        foreach (var (scaleValue, scaleName) in Scales)
        {
            long groupValue = remaining / scaleValue;
            remaining %= scaleValue;

            if (groupValue == 0)
            {
                continue;
            }

            string groupWords = ConvertUnderOneThousand(groupValue);
            if (!string.IsNullOrEmpty(scaleName))
            {
                groupWords = $"{groupWords} {scaleName}";
            }

            groups.Add((groupValue, groupWords));
        }

        return JoinGroups(groups);
    }

    private static string ConvertUnderOneThousand(long number)
    {
        if (number <= 0 || number > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        var parts = new List<string>();
        long hundreds = number / 100;
        long remainder = number % 100;

        if (hundreds > 0)
        {
            parts.Add($"{Ones[hundreds]} hundred");
        }

        if (remainder > 0)
        {
            if (hundreds > 0)
            {
                parts.Add("and");
            }

            parts.Add(ConvertUnderOneHundred(remainder));
        }

        return string.Join(' ', parts);
    }

    private static string ConvertUnderOneHundred(long number)
    {
        if (number < 20)
        {
            return Ones[number];
        }

        long tens = number / 10;
        long ones = number % 10;

        if (ones == 0)
        {
            return Tens[tens];
        }

        return $"{Tens[tens]}-{Ones[ones]}";
    }

    private static string JoinGroups(List<(long Value, string Words)> groups)
    {
        if (groups.Count == 0)
        {
            return Ones[0];
        }

        if (groups.Count == 1)
        {
            return groups[0].Words;
        }

        string output = groups[0].Words;

        for (int i = 1; i < groups.Count; i++)
        {
            bool isLast = i == groups.Count - 1;
            long value = groups[i].Value;
            string words = groups[i].Words;

            if (isLast && value < 100)
            {
                output += $" and {words}";
            }
            else
            {
                output += $", {words}";
            }
        }

        return output;
    }

    private static string CapitaliseFirstLetter(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpper(value[0], CultureInfo.InvariantCulture) + value[1..];
    }
}
