using MiniBlog.Data;

namespace MiniBlog.Features.Posts;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/posts").WithTags("Posts");

        group.MapGet("/", () => Results.Ok(BlogStorage.Posts));

        group.MapGet("/{id:int}", (int id) =>
        {
            var post = BlogStorage.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null) return Results.NotFound();

            return Results.Ok(post);
        });

        group.MapGet("/{slug}", (string slug) =>
        {
            var post = BlogStorage.Posts.FirstOrDefault(p => p.Slug.ToLower() == slug.ToLower());

            if (post == null) return Results.NotFound();

            return Results.Ok(post);
        });

        group.MapGet("/q", (string search) =>
        {
            var found = BlogStorage.Posts.Where(p => p.Title.ToLower().Contains(search.ToLower()) || p.Content.ToLower().Contains(search.ToLower())).ToList();

            return Results.Ok(found);
        });

        group.MapGet("/category/{categorySlug}", (string categorySlug) =>
        {
            var filter = BlogStorage.Posts.Where(p => p.Categories.Any(c => c.Slug.ToLower() == categorySlug.ToLower())).ToList();

            return Results.Ok(filter);
        });

        group.MapPost("/", (CreatePostRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title)) return Results.BadRequest("Title обов'язковий");

            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                ImageUrl = request.ImageUrl,
                Slug = request.Title.ToLower().Replace(" ", "-")
            };

            if (BlogStorage.Posts.Any(p => p.Slug.ToLower() == post.Slug.ToLower())) return Results.Conflict($"Пост з таким slug ({post.Slug}) уже існує");
            
            foreach (var categoryId in request.CategoryIds)
            {
                var category = BlogStorage.Categories.FirstOrDefault(c => c.Id == categoryId);

                if (category == null)
                    return Results.BadRequest($"Категорії з id: {categoryId} не існує!!!");

                post.Categories.Add(category);
            }

            var newPost = BlogStorage.AddPost(post);

            return Results.Ok(newPost);
        });

        group.MapPatch("/{id:int}", (int id, Post updatePost) =>
        {
            var post = BlogStorage.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null) return Results.NotFound();

            post.Title = string.IsNullOrWhiteSpace(updatePost.Title) ? post.Title : updatePost.Title;
            post.Content = string.IsNullOrWhiteSpace(updatePost.Content) ? post.Content : updatePost.Content;
            post.ImageUrl = string.IsNullOrWhiteSpace(updatePost.ImageUrl) ? post.ImageUrl : updatePost.ImageUrl;

            return Results.Ok(post);
        });

        group.MapDelete("/{id:int}", (int id) =>
        {
            var post = BlogStorage.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null) return Results.NotFound();

            BlogStorage.Posts.Remove(post);
            return Results.NoContent();
        });
    }
}
