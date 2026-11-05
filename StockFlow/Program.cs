using StockFlow;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new List<Product>();

var newProduct1 = new Product
{
    Id = 1,
    Name = "book",
    Price = 100,
    Count = 3,
    Descriptions = "BLALALAL",
    ImageUrl = "https://www.google.com/url?sa=t&source=web&rct=j&url=https%3A%2F%2Fes.pinterest.com%2Fpin%2F1145743961435445972%2F&ved=0CBcQjRxqGAoTCOC5ycbS8JcDFQAAAAAdAAAAABDQAQ&opi=89978449"
};

var newProduct2 = new Product
{
    Id = 2,
    Name = "gun",
    Price = 3920,
    Count = 1,
    Descriptions = "PIUU PAUU"
};

products.AddRange(newProduct1, newProduct2);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        await context.Response.SendFileAsync("index.html");
        return;
    }

    if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH" || context.Request.Method == "DELETE")
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || apiKey != "my-secret-key")
        {
            context.Response.StatusCode = 401;
            return;
        }
    }

    if (context.Request.Path == "/products" && context.Request.Method == "GET")
    {
        foreach (var product in products)
        {
            await context.Response.WriteAsync($"" +
                $"Id: {product.Id}\n" +
                $"Name: {product.Name}\n" +
                $"Price: {product.Price}\n" +
                $"Count: {product.Count}\n" +
                $"Description: {product.Descriptions}\n" +
                $"Image: {product.ImageUrl}\n\n");
        }

        context.Response.StatusCode = 200;
        return;
    }

    if (context.Request.Path.StartsWithSegments("/products") && context.Request.Method == "GET")
    {
        string path = context.Request.Path.ToString();
        var segments = path.Split("/");

        string strId = segments[2];
        int id = int.Parse(strId);

        var product = products.FirstOrDefault(p => p.Id == id);


        if (product != null)
            await context.Response.WriteAsync($"" +
                $"Id: {product.Id}\n" +
                $"Name: {product.Name}\n" +
                $"Price: {product.Price}\n" +
                $"Count: {product.Count}\n" +
                $"Description: {product.Descriptions}\n" +
                $"Image: {product.ImageUrl}\n\n");

        context.Response.StatusCode = 200;
        return;
    }

    if (context.Request.Path == "/products" && context.Request.Method == "POST")
    {
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();

        var newProduct = JsonSerializer.Deserialize<Product>(body);

        if (string.IsNullOrWhiteSpace(newProduct.Name) || newProduct.Count <= 0 || newProduct.Price <= 0)
        {
            context.Response.StatusCode = 400;
            return;
        }

        products.Add(newProduct);

        context.Response.StatusCode = 201;
        await context.Response.WriteAsync("Product successfully created!");
        return;
    }

    if (context.Request.Path.StartsWithSegments("/products") && context.Request.Method == "PUT") 
    {
        string path = context.Request.Path.ToString();
        var segments = path.Split("/");

        string strId = segments[2];
        int id = int.Parse(strId);

        var product = products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            var newProduct = JsonSerializer.Deserialize<Product>(body);

            if (string.IsNullOrWhiteSpace(newProduct.Name) || newProduct.Count <= 0 || newProduct.Price <= 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            product.Name = newProduct.Name;
            product.Price = newProduct.Price;
            product.Count = newProduct.Count;
            product.Descriptions = newProduct.Descriptions;
        }
        return;
    }

    if (context.Request.Path.StartsWithSegments("/products") && context.Request.Method == "PATCH") 
    {
        string path = context.Request.Path.ToString();
        var segments = path.Split("/");

        string strId = segments[2];
        int id = int.Parse(strId);

        var product = products.FirstOrDefault(p => p.Id == id);

        if (product != null)
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            var newProduct = JsonSerializer.Deserialize<Product>(body);

            product.Name = newProduct.Name;
        }

        return;
    }

    if (context.Request.Path.StartsWithSegments("/products") && context.Request.Method == "DELETE")
    {
        string path = context.Request.Path.ToString();
        var segments = path.Split("/");

        string strId = segments[2];
        int id = int.Parse(strId);

        var product = products.FirstOrDefault(p => p.Id == id);

        if (product != null) products.Remove(product);

        context.Response.StatusCode = 204;
        return;
    }

    await next();
});

app.Run();
