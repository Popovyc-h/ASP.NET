namespace MiniBlog.Features.Posts;

public class CreatePostRequest
{
    public List<int> CategoryIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

}