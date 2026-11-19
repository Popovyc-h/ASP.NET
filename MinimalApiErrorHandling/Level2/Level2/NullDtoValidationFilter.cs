namespace Level2;

public class NullDtoValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.Any(arg => arg is null))
        {
            return Results.BadRequest(new
            {
                title = "Invalid request payload",
                detail = "Request body or required parameter cannot be null."
            });
        }

        return await next(context);
    }
}
