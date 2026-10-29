namespace HPM.Web.Data;

public static class CurrencyWordsHelper
{
    private static readonly string[] Units =
    [
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    ];

    private static readonly string[] Tens =
    [
        "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    ];

    public static string ToTakaWords(decimal amount)
    {
        if (amount < 0) return "Minus " + ToTakaWords(Math.Abs(amount));
        long integerPart = (long)Math.Truncate(amount);
        int decimalPart = (int)Math.Round((amount - integerPart) * 100);

        if (integerPart == 0 && decimalPart == 0) return "Taka Zero Only";

        string words = integerPart > 0 ? "Taka " + ConvertNumberToWords(integerPart) : "";
        if (decimalPart > 0)
        {
            if (string.IsNullOrWhiteSpace(words))
                words = $"{ConvertNumberToWords(decimalPart)} Paisa Only";
            else
                words += $" and {ConvertNumberToWords(decimalPart)} Paisa Only";
        }
        else
        {
            words += " Only";
        }

        return words;
    }

    private static string ConvertNumberToWords(long number)
    {
        if (number == 0) return "Zero";
        if (number < 0) return "Minus " + ConvertNumberToWords(Math.Abs(number));

        var sb = new System.Text.StringBuilder();

        // 1 Crore = 10,000,000
        if (number >= 10000000)
        {
            sb.Append(ConvertNumberToWords(number / 10000000) + " Crore ");
            number %= 10000000;
        }

        // 1 Lakh = 100,000
        if (number >= 100000)
        {
            sb.Append(ConvertNumberToWords(number / 100000) + " Lakh ");
            number %= 100000;
        }

        // 1 Thousand = 1,000
        if (number >= 1000)
        {
            sb.Append(ConvertNumberToWords(number / 1000) + " Thousand ");
            number %= 1000;
        }

        // 1 Hundred = 100
        if (number >= 100)
        {
            sb.Append(ConvertNumberToWords(number / 100) + " Hundred ");
            number %= 100;
        }

        if (number > 0)
        {
            if (number < 20)
            {
                sb.Append(Units[number]);
            }
            else
            {
                sb.Append(Tens[number / 10]);
                if (number % 10 > 0)
                {
                    sb.Append(" " + Units[number % 10]);
                }
            }
        }

        return sb.ToString().Trim();
    }
}
