namespace MiniBlog.Features.Categories;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}
