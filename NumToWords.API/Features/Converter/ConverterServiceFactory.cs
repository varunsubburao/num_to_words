namespace NumToWords.API.Features.Converter;

public class ConverterServiceFactory(ConverterServiceEnglish converterServiceEnglish, ConverterServiceGerman converterServiceGerman)
{

    public ConverterServiceBase CreateConverterService(string language)
    {
        if (string.Equals(language, "english", StringComparison.OrdinalIgnoreCase))
            return converterServiceEnglish;
        else if (string.Equals(language, "german", StringComparison.OrdinalIgnoreCase))
            return converterServiceGerman;
        else
            throw new ArgumentException("Invalid language");
    }
}