using AspNetCore.Scalar;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
// Add services to the container. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSwagger();
app.UseScalar(options =>
{
    options.UseTheme(Theme.Default);
    options.RoutePrefix = "api-docs";
});
//app.MapOpenApi();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapScalarApiReference();
//}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (string? email) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
//.AddResponseExample(200, typeof(string))

.WithOpenApi(GenerateOperation());

app.Run();

//public static class OpenApiExtensions
//{
//    public static TBuilder AddResponseExample<TBuilder>(this TBuilder builder, Func<OpenApiOperation, OpenApiOperation> configureOperation, int statusCode, Type type)
//        where TBuilder : IEndpointConventionBuilder
//    {
//        return builder;
//    }
//}

static Func<OpenApiOperation, OpenApiOperation> GenerateOperation()
{
    return operation =>
    {
        // Summary
        operation.Summary = "Endpoint used to fetch users";
        operation.Description = "This is a description";

        // Parameters
        var parameter = operation.Parameters[0];
        parameter.Description = "The user e-mail.It will filter the users by using this value";
        parameter.Required = false;
        parameter.AllowEmptyValue = true;
        parameter.Examples = new Dictionary<string, OpenApiExample>()
        {
            {
                "talles.valiatt@tallesvaliatti.com",
                new OpenApiExample {
                    Value = new OpenApiString("talles.valiatt@tallesvaliatti.com"),
                Description = "It will filter users who have their email as 'talles.valatt@tallesvaliatti.com'" } },
            {
             "talles", new OpenApiExample { Value = new OpenApiString("talles"),
                 Description = "It will filter users if their email contains the word 'talles'" } }
        };

        operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse
            {
                Description = "Resultado da conversão (com valores em Celsius e Kelvin)",
            },
            ["400"] = new OpenApiResponse
            {
                Description = "Temperatura em Fahrenheit inválida"
            },
            //["200"] = new OpenApiResponse { Description = "Success" } ,
            //["400"] = new OpenApiResponse { Description = "Bad Request" },
            ["500"] = new OpenApiResponse
            {
                Description = "Internal Server Error"
            }
        };
        return operation;
    };
}
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}