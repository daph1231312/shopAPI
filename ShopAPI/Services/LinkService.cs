using Microsoft.AspNetCore.Http;

namespace ShopAPI.Services;

/// <summary>
/// Generates HATEOAS _links for each resource type.
/// Links tell API clients what actions are available next — no out-of-band docs needed.
/// </summary>
public class LinkService : ILinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Requires IHttpContextAccessor to build absolute URLs.</summary>
    public LinkService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    private string BaseUrl
    {
        get
        {
            var req = _httpContextAccessor.HttpContext?.Request;
            return req is null ? "" : $"{req.Scheme}://{req.Host}";
        }
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetProductLinks(int productId) => new()
    {
        ["self"]   = $"{BaseUrl}/api/products/{productId}",
        ["update"] = $"{BaseUrl}/api/products/{productId}",
        ["delete"] = $"{BaseUrl}/api/products/{productId}",
        ["all"]    = $"{BaseUrl}/api/products"
    };

    /// <inheritdoc/>
    public Dictionary<string, string> GetCategoryLinks(int categoryId) => new()
    {
        ["self"]     = $"{BaseUrl}/api/categories/{categoryId}",
        ["delete"]   = $"{BaseUrl}/api/categories/{categoryId}",
        ["products"] = $"{BaseUrl}/api/products?categoryId={categoryId}",
        ["all"]      = $"{BaseUrl}/api/categories"
    };

    /// <inheritdoc/>
    public Dictionary<string, string> GetOrderLinks(int orderId) => new()
    {
        ["self"]          = $"{BaseUrl}/api/orders/{orderId}",
        ["update-status"] = $"{BaseUrl}/api/orders/{orderId}/status",
        ["all"]           = $"{BaseUrl}/api/orders"
    };
}
