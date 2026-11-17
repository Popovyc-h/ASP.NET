using MiniBlog.Data;
using Scalar.AspNetCore;

namespace MiniBlog.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories").WithTags("Categories");

        group.MapGet("/", () =>
        {
            var result = BlogStorage.Categories.Select(c => new CategoryResponse { Id = c.Id, Name = c.Name, Slug = c.Slug, Link = $"/posts/category/{c.Slug}" }).ToList();

            return Results.Ok(result);
        })
            .WithName("GetAllCategory")
            .WithSummary("Отримати список усіх категорій");

        group.MapPost("/", (Category category) =>
        {
            if (string.IsNullOrWhiteSpace(category.Name) || string.IsNullOrWhiteSpace(category.Slug)) return Results.BadRequest("Параметри Name, Slug є обов'язковими");

            if (BlogStorage.Categories.Any(c => c.Slug.ToLower() == category.Slug.ToLower())) return Results.Conflict($"Категорія з таким slug ({category.Slug}) уже існує");

            var newCategory = BlogStorage.AddCategory(category);

            return Results.Ok(newCategory);
        })
            .WithName("CreateCategory")
            .WithSummary("Створити нову категорію");

        group.MapPatch("/{id:int}", (int id, string name) =>
        {
            var category = BlogStorage.Categories.FirstOrDefault(c => c.Id == id);

            if (string.IsNullOrWhiteSpace(name)) return Results.BadRequest("Name не може бути порожнім");

            if (category == null) return Results.NotFound();

            category.Name = name;
            return Results.Ok(category);
        })
            .WithName("EditCategoryName")
            .WithSummary("Редагувати назву категорії");

        group.MapDelete("/{id:int}", (int id) =>
        {
            var category = BlogStorage.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null) return Results.NotFound();

            BlogStorage.Categories.Remove(category);
            return Results.NoContent();
        })
            .WithName("DeleteCategory")
            .WithSummary("Видалити категорію за ID");
    }
}
