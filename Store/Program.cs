using Store.Features.Products;
using Store.Features.Cart;
using Store.Features.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();
app.UseExceptionHandler();

app.UseSession();

var products = new List<Product>
{
    new() {Id = 1, Name = "Ноутбук", Price = 25000.00m},
    new() {Id = 2, Name = "Мишка", Price = 750.00m},
    new() {Id = 3, Name = "Клавіатура", Price = 1599.00m},
    new() {Id = 4, Name = "Монітор", Price = 9000.00m},
    new() {Id = 5, Name = "Навушники", Price = 2000.00m}
};

app.Logger.LogInformation("Додаток запущено. Завантажено товарів: {products.Count}", products.Count);

app.MapGet("/identify", (HttpContext context) =>
{
    if (!context.Request.Cookies.ContainsKey("UserId"))
    {
        var newUserId = Guid.NewGuid();
        context.Response.Cookies.Append("UserId", newUserId.ToString());
        app.Logger.LogInformation("Видано новий ID: {newUserId}", newUserId);

        return Results.Ok($"Створено новий ID: {newUserId}");
    }
    else
    {
        if (Guid.TryParse(context.Request.Cookies["UserId"], out var parsedId))
            return Results.Ok();
        else
            return Results.BadRequest("Помилка! Coockie було змінено");
    }
});

app.MapCartEndpoints(products);

app.Run();
