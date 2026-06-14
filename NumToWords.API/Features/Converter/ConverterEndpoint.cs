namespace NumToWords.API.Features.Converter;

public static class ConverterEndpoint
{
    public static void MapConverterEndpoints(this WebApplication app)
    {
        app.MapPost("/convert", ConvertToWords)
            .WithName("ConvertToWords");
    }

    private static IResult ConvertToWords(ConverterRequest converterRequest,
    ConverterServiceFactory converterServiceFactory,
    ILogger<Program> logger)
    {
        logger.LogInformation("Converting number to words for amount: {Amount} and language: {Language}", converterRequest.Amount, converterRequest.Language);
        if (converterRequest == null || converterRequest.Language == null ||
            !(converterRequest.Language.ToLower().Equals("english") || converterRequest.Language.ToLower().Equals("german")))
            return Results.BadRequest(new { message = "Invalid language" });
        if (converterRequest.Amount == null || converterRequest.Amount.Length == 0)
            return Results.BadRequest(new { message = "Invalid amount" });
        var service = converterServiceFactory.CreateConverterService(converterRequest.Language);
        var response = service.ConvertNumberToWords(converterRequest.Amount);
        return response;
    }
}
