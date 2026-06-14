using NumToWords.API.Features.Converter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<ConverterServiceEnglish>();
builder.Services.AddTransient<ConverterServiceGerman>();
builder.Services.AddSingleton<ConverterServiceFactory>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();


app.UseCors("AllowLocalhost");
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello from NumToWords API");

app.MapConverterEndpoints();

app.Run();
