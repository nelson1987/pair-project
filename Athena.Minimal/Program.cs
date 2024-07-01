using AspNetCore.Scalar;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Linq.Expressions;

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
.WithRequestExample(typeof(PersonRequest), typeof(PersonRequestExample))
.WithResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the newly created resource")
.WithResponseHeader(200, "ETag", "string", "An ETag of the resource")
.WithResponse(200, typeof(WeatherForecast), "Successfully found the person")
.WithResponseExample(200, typeof(PersonResponseExample))
.WithResponse(500, type: null, description: "There was an unexpected error")
.WithResponseExample(500, typeof(InternalServerResponseExample))

//.WithRequestExample(typeof(PersonRequest), typeof(PersonRequestExample))]
//.WithResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the newly created resource")]
//.WithResponseHeader(200, "ETag", "string", "An ETag of the resource")]
.WithOpenApi(GenerateOperation());

app.Run();

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

public static class OpenApiExtensions
{
    //public static TBuilder AddResponseExample<TBuilder>(this TBuilder builder, Func<OpenApiOperation, OpenApiOperation> configureOperation, int statusCode, Type type)
    //    where TBuilder : IEndpointConventionBuilder
    //{
    //    return builder;
    //}

    private static void AddAndConfigureOperationForEndpoint(EndpointBuilder endpointBuilder)
    {
        foreach (var item in endpointBuilder.Metadata)
        {
        }
    }

    public static TBuilder WithRequestExample<TBuilder>(this TBuilder builder, Type typeRequest, Type typeExample)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Finally(endpointBuilder => AddAndConfigureOperationForEndpoint(endpointBuilder));
        return builder;
    }

    public static TBuilder WithResponseHeader<TBuilder>(this TBuilder builder, int statusCode, string tag, string type, string description)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder;
    }

    public static TBuilder WithResponseExample<TBuilder>(this TBuilder builder, int statusCode, Type type)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder;
    }

    public static TBuilder WithResponse<TBuilder>(this TBuilder builder, int statusCode, Type? type, string description)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder;
    }
}

public interface IExamplesProvider<T> where T : class
{
    T GetExamples();
}

internal class PersonResponseExample : IExamplesProvider<WeatherForecast>
{
    public WeatherForecast GetExamples()
    {
        return new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Random.Shared.Next(-20, 55),
            "Scorching"
        );
    }
}

internal class InternalServerResponseExample : IExamplesProvider<ErrorResponse>
{
    public ErrorResponse GetExamples()
    {
        return new ErrorResponse { ErrorCode = 500 };
    }
}

public class ErrorResponse
{
    public int ErrorCode { get; set; }

    public string Message { get; set; }
}

public record PersonRequest(string Description);

internal class PersonRequestExample : IExamplesProvider<PersonRequest>
{
    public PersonRequest GetExamples()
    {
        return new PersonRequest("Descrição");
    }
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class PersonRequestDescriptor : AbstractDescriptor<PersonRequest>
{
    public PersonRequestDescriptor()
    {
        DescribeFor(x => x.FirstName).WithName("The first name of the person");
    }
}

/*

                var propNames = requestType.GetProperties()
                    .Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()))
                    .ToList();
 */

public interface IDescriptor<T> where T : class
{
}

public abstract class AbstractDescriptor<T> : IDescriptor<T> where T : class
{
    public void DescribeFor<TProperty>(Expression<Func<T, TProperty>> expression)
    {
    }

    public void WithName(string descriptorMessage)
    {
    }
}