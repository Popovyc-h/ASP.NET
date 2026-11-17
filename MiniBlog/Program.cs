using MiniBlog.Features.Categories;
using MiniBlog.Features.Posts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Mini Blog";
        document.Info.Version = "v1.0";
        document.Info.Description = "Навчальний проєкт ASP.NET Core Minimal API";

        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Mini Blog API";
    options.Theme = ScalarTheme.Kepler;
});

app.UseStaticFiles();

app.MapGet("/about", () =>
{
    return Results.Redirect("/about.html");
}).WithTags("Pages");

app.MapCategoryEndpoints();
app.MapPostEndpoints();

app.Run();
