namespace NumToWords.API.Features.Converter;

public class ConverterServiceEnglish : ConverterServiceBase
{
    public ConverterServiceEnglish(ILogger<ConverterServiceEnglish> logger) : base(logger)
    {
    }

/// <summary>
/// Public Method to convert dollar Amount from string of numbers to string of words in English.
/// example:
///     1. amount: 123,45
///        returns: "one hundred twenty three dollars and forty five cents"
///     2. amount: 1000,00
///        returns: "one thousand dollars"
///     3. amount: 1000,01
///        returns: "one thousand dollars and one cent"
///     4. amount: 5079,10
///        returns: "five thousand seventy nine dollars and ten cents"
///     5. amount: 0,11
///        returns: "zero dollars and eleven cents"
/// </summary>
/// <param name="amount">
///     String of numbers to convert to words in English.
///     example:
///         1. amount: 123,45
///         2. amount: 1000,00
///         3. amount: 1000,01
///         4. amount: 5079,10
///         5. amount: 0,11
/// </param>
/// <returns>
///     String of words in English.
///     example:
///         1. amount: 123,45
///            returns: "one hundred twenty three dollars and forty five cents"
/// </returns>
/// </summary>
    public override IResult ConvertNumberToWords(string amount)
    {
        _logger.LogInformation("Converting number to words in English for amount: {Amount}", amount);
        (string dollarsString, string centsString) = GetDollarsAndCents(amount);
        string? validationError = ValidateAmount(dollarsString, centsString);
        if (validationError != null)
        {
            _logger.LogError("Validation error: {ValidationError}", validationError);
            return Results.BadRequest(new { message = validationError });
        }
        (int dollars, int cents) = ParseAmountString(dollarsString, centsString);
        string dollarsInWords = GetNumbersInWords(dollars);
        string centsInWords = "";
        if (cents != 0)
            centsInWords = GetNumbersInWords(cents);
        string dollarUnit = GetDollarUnit(dollars);
        string centUnit = GetCentUnit(cents);
        string totalInWords = GetTotalInWords(dollarsInWords, centsInWords, dollarUnit, centUnit);
        return Results.Ok(new ConverterResponse(totalInWords));
    }

    protected override string GetNumbersInWords(int number)
    {
        _logger.LogInformation("Getting number in words for number: {Number}", number);
        if (number == 0)
            return "zero";
        
        Dictionary<int, string> tens = GetTensDictionary();
        Dictionary<int, string> digits = GetDigitDictionary();
        Dictionary<int, string> powersOfTen = GetPowersOfTenDictionary();


        string numberString = new string(number.ToString().Reverse().ToArray());
        var numberWordsList = new List<string>();
        int numberIndex = numberString.Length - 1;
        _logger.LogDebug("Starting number index: {NumberIndex}", numberIndex);
        while (numberIndex >= 0)
        {   
            int inHundredPos = numberIndex % 3;
            int powerOfTenPos = numberIndex / 3;
            _logger.LogDebug("In hundred position: {InHundredPos}, Power of ten position: {PowerOfTenPos}", inHundredPos, powerOfTenPos);
            int digit1, digit2, digit3;
            bool digit3Covered = false;
            (digit1, digit2, digit3, numberIndex) = GetDigitsInHundredPos(numberString, numberIndex, inHundredPos);
            int chunkNumber = digit1 * 100 + digit2 * 10 + digit3;
            _logger.LogDebug("Obtained chunk number: {ChunkNumber}", chunkNumber);
            if (chunkNumber != 0)
            {
                if (digit1 != 0)
                    numberWordsList.Add(digits[digit1] + " hundred");
                if (digit2 != 0)
                {
                    if (digit2 != 1)
                        numberWordsList.Add(tens[digit2]);
                    else
                    {
                        numberWordsList.Add(tens[digit2 * 10 + digit3]);
                        digit3Covered = true;
                    }
                }
                if (!digit3Covered && digit3 != 0)
                    numberWordsList.Add(digits[digit3]);
                if (powerOfTenPos > 0)
                    numberWordsList.Add(powersOfTen[powerOfTenPos]);
            }
        }
        string numberInWords = string.Join(" ", numberWordsList);
        _logger.LogDebug("Number in words: {NumberInWords}", numberInWords);
        return numberInWords;
        
    }

    protected override Dictionary<int, string> GetDigitDictionary()
    {
        return new Dictionary<int, string>
        {
            {0, ""},
            {1, "one"},
            {2, "two"},
            {3, "three"},
            {4, "four"},
            {5, "five"},
            {6, "six"},
            {7, "seven"},
            {8, "eight"},
            {9, "nine"}            
        };
    }

    protected override Dictionary<int, string> GetTensDictionary()
    {
        return new Dictionary<int, string>
        {
            {10, "ten"},
            {11, "eleven"},
            {12, "twelve"},
            {13, "thirteen"},
            {14, "fourteen"},
            {15, "fifteen"},
            {16, "sixteen"},
            {17, "seventeen"},
            {18, "eighteen"},
            {19, "nineteen"},
            {2, "twenty"},
            {3, "thirty"},
            {4, "forty"},
            {5, "fifty"},
            {6, "sixty"},
            {7, "seventy"},
            {8, "eighty"},
            {9, "ninety"},
            {0, ""}
        };
    }

    protected override Dictionary<int, string> GetPowersOfTenDictionary()
    {
        return new Dictionary<int, string>
        {
            {0, ""},
            {1, "thousand"},
            {2, "million"}
        };
    }

    protected override string GetDollarUnit(int dollars)
    {
        _logger.LogDebug("Getting dollar unit for dollars: {Dollars}", dollars);
        string dollarUnit;
        if (dollars == 1)
            dollarUnit = "dollar";
        else
            dollarUnit = "dollars";
        _logger.LogDebug("Dollar unit: {DollarUnit}", dollarUnit);
        return dollarUnit;
    }

    protected override string GetCentUnit(int cents)
    {
        _logger.LogDebug("Getting cent unit for cents: {Cents}", cents);
        string centUnit;    
        if (cents == 0)
            centUnit = "";
        else if (cents == 1)
            centUnit = "cent";
        else
            centUnit = "cents";
        _logger.LogDebug("Cent unit: {CentUnit}", centUnit);
        return centUnit;
    }

    protected override string GetTotalInWords(string dollarsInWords, string centsInWords, string dollarUnit, string centUnit)
    {
        if (centsInWords == "")
            return $"{dollarsInWords} {dollarUnit}";
        else
            return $"{dollarsInWords} {dollarUnit} and {centsInWords} {centUnit}";
    }
}
