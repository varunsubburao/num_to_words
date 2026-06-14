namespace NumToWords.API.Features.Converter;


public class ConverterServiceGerman : ConverterServiceBase
{
    public ConverterServiceGerman(ILogger<ConverterServiceGerman> logger) : base(logger)
    {
    }

/// <summary>
/// Public Method to convert dollar Amount from string of numbers to string of words in German.
/// example:
///     1. amount: 123,45
///        returns: "einhundertdreiundzwanzig dollar und fünfundvierzig cent"
///     2. amount: 1000,00
///        returns: "eintausend dollar"
///     3. amount: 1000,01
///        returns: "eintausend dollar und ein cent"
///     4. amount: 1000,10
///        returns: "eintausend dollar und zehn cent"
///     5. amount: 0,11
///        returns: "null dollar und elf cent"
/// </summary>
/// <param name="amount">
///     String of numbers to convert to words in German.
///     example:
///         1. amount: 123,45
///         2. amount: 1000,00
///         3. amount: 1000,01
///         4. amount: 1000,10
///         5. amount: 0,11
/// </param>
/// <returns>
///     String of words in German.
///     example:
///         1. amount: 123,45
///            returns: "einhundertdreiundzwanzig dollar und fünfundvierzig cent"
/// </returns>
/// </summary>
    public override IResult ConvertNumberToWords(string amount)
    {
        _logger.LogInformation("Converting number to words in German for amount: {Amount}", amount);
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
        if (number == 0)
            return "null";

        Dictionary<int, string> digits = GetDigitDictionary();
        Dictionary<int, string> tens = GetTensDictionary();
        Dictionary<int, string> powersOfTen = GetPowersOfTenDictionary();

        string numberString = new string(number.ToString().Reverse().ToArray());
        _logger.LogDebug("Number string: {NumberString}", numberString);
        int numberIndex = numberString.Length - 1;
        var numberWordsList = new List<string>();
        while (numberIndex >= 0)
        {
            int inHundredPos = numberIndex % 3;
            int powerOfTenPos = numberIndex / 3;
            int digit1, digit2, digit3;
            (digit3, digit2, digit1, numberIndex) = GetDigitsInHundredPos(numberString, numberIndex, inHundredPos);
            int chunkNumber = digit3 * 100 + digit2 * 10 + digit1;
            _logger.LogDebug("Obtained chunk number: {ChunkNumber}", chunkNumber);
            if (chunkNumber != 0)
            {
                if (chunkNumber == 1) // Special case for 1. ein and eine
                {
                    if (powerOfTenPos == 2)
                        numberWordsList.Add("eine"); // Million is female. So eine
                    else
                        numberWordsList.Add("ein"); // Rest of the 10 power numbers are male. So ein
                }
                else 
                {
                    if (digit3 != 0)
                        numberWordsList.Add(digits[digit3] + "hundert");
                    if (digit2 == 1) // Special case for 10-19. zehn, elf, zwölf, etc.
                        numberWordsList.Add(tens[digit2 * 10 + digit1]);
                    else if (digit2 != 0)
                    {
                        if (digit1 == 0)
                            numberWordsList.Add(tens[digit2]); // In Case digit1 is 0, No "und" needed. ex: 20, 30, 40, etc.
                        else
                            numberWordsList.Add(digits[digit1] + "und" + tens[digit2]); // "und" is needed for all other numbers. ex: 21 => 1 and 20, 22, 23, etc.
                    }
                    else
                    {
                        if (digit1 != 0)
                            numberWordsList.Add(digits[digit1]);
                    }
                    
                }
                if (powerOfTenPos == 2 && chunkNumber == 1)
                        numberWordsList.Add(" Million ");
                else if (powerOfTenPos > 0)
                    numberWordsList.Add(powersOfTen[powerOfTenPos]);
                
            }
           
        }
        string numberInWords = string.Join("", numberWordsList);
        _logger.LogDebug("Number in words: {NumberInWords}", numberInWords);
        return numberInWords;
    }

    protected override Dictionary<int, string> GetDigitDictionary()
    {
        return new Dictionary<int, string>
        {
            {0, ""},
            {1, "ein"},
            {2, "zwei"},
            {3, "drei"},
            {4, "vier"},
            {5, "fünf"},
            {6, "sechs"},
            {7, "sieben"},
            {8, "acht"},
            {9, "neun"}
        };
    }
    protected override Dictionary<int, string> GetTensDictionary()
    {
        return new Dictionary<int, string>
        {
            {10, "zehn"},
            {11, "elf"},
            {12, "zwölf"},
            {13, "dreizehn"},
            {14, "vierzehn"},
            {15, "fünfzehn"},
            {16, "sechzehn"},
            {17, "siebzehn"},
            {18, "achtzehn"},
            {19, "neunzehn"},
            {2, "zwanzig"},
            {3, "dreißig"},
            {4, "vierzig"},
            {5, "fünfzig"},
            {6, "sechzig"},
            {7, "siebzig"},
            {8, "achtzig"},
            {9, "neunzig"}
        };
    }

    protected override Dictionary<int, string> GetPowersOfTenDictionary()
    {
        return new Dictionary<int, string>
        {
            {0, ""},
            {1, "tausend"},
            {2, " Millionen "},
        };
    }

    protected override string GetDollarUnit(int dollars)
    {
        return "dollar"; // Just singular form for German.
    }

    protected override string GetCentUnit(int cents)
    {
        if (cents == 0)
            return "";
        return "cent"; // Just singular form for German.
    }

    protected override string GetTotalInWords(string dollarsInWords, string centsInWords, string dollarUnit, string centUnit)
    {
        string totalInWords;
        if (centsInWords == "")
            totalInWords = $"{dollarsInWords} {dollarUnit}";
        else
            totalInWords = dollarsInWords + " " + dollarUnit + " und " + centsInWords + " " + centUnit;
        _logger.LogDebug("Total in words: {TotalInWords}", totalInWords);
        return totalInWords;
    }


}