var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    };
});

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/error", () =>
{
    throw new Exception();
});

app.Run();


/*
Майстер статусів:

1. Користувач надіслав порожній JSON. - 400 Bad Request

2 - Користувач намагається видалити товар, якого не існує. - 404 Not Found

3 - У базі даних стався збій під час запису. - 500 Internal Server Error

4 - Клієнт робить 100 запитів на секунду при ліміті 10. - 429 Too Many Requests

5 - Користувач не надав Bearer-токен. - 401 Unauthorized
*/