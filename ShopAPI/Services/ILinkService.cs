namespace ShopAPI.Services;

/// <summary>
/// Builds HATEOAS hypermedia links for API responses.
/// This is what pushes us to Richardson Maturity Level 4.
/// </summary>
public interface ILinkService
{
    /// <summary>Returns links for a product resource.</summary>
    Dictionary<string, string> GetProductLinks(int productId);

    /// <summary>Returns links for a category resource.</summary>
    Dictionary<string, string> GetCategoryLinks(int categoryId);

    /// <summary>Returns links for an order resource.</summary>
    Dictionary<string, string> GetOrderLinks(int orderId);
}
