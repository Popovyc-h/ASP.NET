using MiniBlog.Features.Categories;
using MiniBlog.Features.Posts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference();

app.UseStaticFiles();

app.MapGet("/about", () =>
{
    return Results.Redirect("/about.html");
}).WithTags("Pages");

app.MapCategoryEndpoints();
app.MapPostEndpoints();

app.Run();
