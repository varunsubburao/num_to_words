namespace NumToWords.API.Features.Converter;

public record ConverterRequest(string Amount, string Language);

public record ConverterResponse(string AmountInWords);
