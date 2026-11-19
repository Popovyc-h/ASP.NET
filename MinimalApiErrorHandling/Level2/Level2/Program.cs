using Level2;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (HttpContext context, RequestDelegate next) =>
{
    try
    {
        await next(context);
    }
    catch(Exception ex)
    {
        app.Logger.LogError(ex, "Необроблена помилка. TraceId: {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            message = $"Сталася внутрішня помилка сервера. TraceId: {context.TraceIdentifier}"
        });
    }
});

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Маршрут не знайдено, перевірте документацію",
            statusCode = 404
        });
    }
});

app.MapPost("/items", (CreateItemDto? dto) => Results.Ok(dto)).AddEndpointFilter<NullDtoValidationFilter>();

app.Run();
