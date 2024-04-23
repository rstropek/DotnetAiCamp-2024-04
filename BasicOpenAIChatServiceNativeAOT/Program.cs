using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
// using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(option =>
{
    option.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine([
        DtoSerializerContext.Default,
        // ...
    ]);
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () => new Person("John", 30));

app.Run();

// static partial class RegExDemo
// {
//     [GeneratedRegex("abc|def", RegexOptions.IgnoreCase, "en-US")]
//     private static partial Regex AbcOrDefGeneratedRegex();
// }

record Person(string Name, int Age);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Person))]
partial class DtoSerializerContext : JsonSerializerContext { }
