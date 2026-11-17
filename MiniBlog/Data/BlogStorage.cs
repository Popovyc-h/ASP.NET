using MiniBlog.Features.Categories;
using MiniBlog.Features.Posts;

namespace MiniBlog.Data;

public static class BlogStorage
{
    public static List<Post> Posts { get; set; } = new();
    public static List<Category> Categories { get; set; } = new();
    public static int PostCounter { get; set; } = 0;
    public static int CategoryCounter { get; set; } = 0;

    public static Category AddCategory(Category category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));

        CategoryCounter++;

        category.Id = CategoryCounter;

        Categories.Add(category);

        return category;
    }

    public static Post AddPost(Post post)
    {
        if (post == null) throw new ArgumentNullException(nameof(post));

        PostCounter++;

        post.Id = PostCounter;
        post.CreatedAt = DateTime.UtcNow;

        Posts.Add(post);

        return post;
    }
}
