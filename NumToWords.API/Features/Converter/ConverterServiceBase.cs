namespace NumToWords.API.Features.Converter;

public abstract class ConverterServiceBase
{
    protected readonly ILogger _logger;

    public ConverterServiceBase(ILogger logger)
    {
        _logger = logger;
    }

    protected string? ValidateAmount(string dollarsString, string centsString)
    {
        if (dollarsString == null || dollarsString.Length == 0)
            return "Dollar string is required";
        if (centsString == null || centsString.Length == 0)
            return "Cents string is required";
        if (!dollarsString.All(char.IsDigit))
            return "Dollar string contains non-digit characters";
        if (!centsString.All(char.IsDigit))
            return "Cents string contains non-digit characters";
        if (int.Parse(dollarsString) > 999999999)
            return "Dollar value is too large";
        if (int.Parse(centsString) > 99)
            return "Cents value is too large";
        return null;
    }

    protected (int dollars, int cents) ParseAmountString(string dollarsString, string centsString)
    {
        int dollars = int.Parse(dollarsString);
        int cents = int.Parse(centsString);
        return (dollars, cents);
    }

    protected (string dollarsString, string centsString) GetDollarsAndCents(string amount)
    {
        _logger.LogDebug("Getting dollars and cents for amount: {Amount}", amount);
        String[] amountSplit = amount.Replace(" ", "").Split(',');
        string dollars = amountSplit[0].Replace(" ", "");
        string cents = "0";
        if (amountSplit.Length > 1)
            cents = amountSplit[1].Replace(" ", "");
        _logger.LogDebug("Amount Split :: Dollars: {Dollars}, Cents: {Cents}", dollars, cents);
        return (dollars, cents);
    }

    protected (int digit1, int digit2, int digit3, int index) GetDigitsInHundredPos(string dollarString, int dollarsIndex, int inHundredPos)
    {
        int digit1, digit2, digit3;
        if (inHundredPos == 2)
        {
            digit1 = int.Parse(dollarString[dollarsIndex].ToString());
            digit2 = int.Parse(dollarString[dollarsIndex - 1].ToString());
            digit3 = int.Parse(dollarString[dollarsIndex - 2].ToString());
            dollarsIndex -= 3;
        }
        else if (inHundredPos == 1)
        {
            digit1 = 0;
            digit2 = int.Parse(dollarString[dollarsIndex].ToString());
            digit3 = int.Parse(dollarString[dollarsIndex - 1].ToString());
            dollarsIndex -= 2;
        }
        else
        {
            digit1 = 0;
            digit2 = 0;
            digit3 = int.Parse(dollarString[dollarsIndex].ToString());
            dollarsIndex -= 1;
        }
        _logger.LogDebug("Obtained digit1: {Digit1}, digit2: {Digit2}, digit3: {Digit3}, index: {Index}",
            digit1, digit2, digit3, dollarsIndex);
        return (digit1, digit2, digit3, dollarsIndex);
    }

    protected abstract string GetNumbersInWords(int numbers);
    protected abstract string GetDollarUnit(int dollars);
    protected abstract string GetCentUnit(int cents);
    protected abstract string GetTotalInWords(string dollarsInWords, string centsInWords, string dollarUnit, string centUnit);
    protected abstract Dictionary<int, string> GetDigitDictionary();
    protected abstract Dictionary<int, string> GetTensDictionary();
    protected abstract Dictionary<int, string> GetPowersOfTenDictionary();
    public abstract IResult ConvertNumberToWords(string amount);
}
