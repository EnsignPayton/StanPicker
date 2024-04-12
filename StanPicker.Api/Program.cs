using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

// Fake in-memory DB
List<string> names = [];

var namesApi = app.MapGroup("/names");
namesApi.MapGet("/", () => names);
namesApi.MapPut("/{name}", (string name) =>
{
    if (!names.Contains(name))
        names.Add(name);
});
namesApi.MapPost("/", (List<string> postNames) => names = postNames);
namesApi.MapDelete("/{name}", (string name) =>
{
    names.Remove(name);
});
namesApi.MapGet("/pick", () => names[Random.Shared.Next(names.Count)]);

app.Run();

[JsonSerializable(typeof(List<string>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
