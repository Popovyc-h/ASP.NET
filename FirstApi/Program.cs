using Microsoft.AspNetCore.Mvc.RazorPages;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int counter = 0;

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/counter")
    {
        counter++;
        await context.Response.WriteAsync($"Count: {counter}");
    }

    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/time")
    {
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        await context.Response.WriteAsync($"Current Time: {currentTime}");
        return;
    }

    await next();
});

app.Use(async (context, next) =>
{
    var items = new List<string>
    {
        "1", "2", "3", "4", "5"
    };

    if (context.Request.Path == "/items")
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        string htmlResponse = "<ul>";

        foreach (var item in items)
        {
            htmlResponse += $"<li>{item}</li>";
        }

        htmlResponse += "</ul>";

        await context.Response.WriteAsync(htmlResponse);
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/headers")
    {
        var headers = context.Request.Headers;
        foreach (var header in headers)
        {
            await context.Response.WriteAsync(header.Key + " - " + header.Value + '\n');
            return;
        }
    }

    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/profile")
    {
        string name = context.Request.Query["name"];
        string surname = context.Request.Query["surname"];
        string age = context.Request.Query["age"];
        string hobby = context.Request.Query["hobby"];
        string job = context.Request.Query["job"];
        string city = context.Request.Query["city"];

        context.Response.ContentType = "text/html; charset=utf-8";

        string profileHtml = $@"
    <div>
        <h2>{name} {surname}</h2>
        <p><b>Вік:</b> {age}</p>
        <p><b>Хобі:</b> {hobby}</p>
        <p><b>Професія:</b> {job}</p>
        <p><b>Місто:</b> {city}</p>
    </div>";

        await context.Response.WriteAsync(profileHtml);
        return;
    }
    await next();
});

app.Run();
