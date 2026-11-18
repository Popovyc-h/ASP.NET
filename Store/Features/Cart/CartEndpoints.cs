using Store.Features.Products;
using System.Text.Json;

namespace Store.Features.Cart;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder endpoints, List<Product> products)
    {
        var cart = endpoints.MapGroup("/cart");

        cart.MapGet("/", (HttpContext ctx) =>
        {
            var cart = ctx.Session.GetString("Cart");

            if (string.IsNullOrWhiteSpace(cart) || cart == null) return Results.Ok(new List<CartItem>());

            var cartJson = JsonSerializer.Deserialize<List<CartItem>>(cart);

            return Results.Ok(cartJson);
        });

        cart.MapPost("/add/{productId:int}", (int productId, HttpContext ctx) =>
        {
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return Results.NotFound();

            var cartJson = ctx.Session.GetString("Cart");
            var userCart = string.IsNullOrWhiteSpace(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;
            
            var existingItem = userCart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity++;
            else
                userCart.Add(new CartItem { ProductId = productId, Quantity = 1 });

            var updatedCartJson = JsonSerializer.Serialize(userCart);

            ctx.Session.SetString("Cart", updatedCartJson);

            return Results.Ok(userCart);
        });

        cart.MapPost("/clear", (HttpContext ctx) =>
        {
            ctx.Session.Remove("Cart");
            return Results.Ok();
        });

        return endpoints;
    }
}
