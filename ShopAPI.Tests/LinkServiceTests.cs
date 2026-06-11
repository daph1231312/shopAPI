using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ShopAPI.Services;
using Xunit;

namespace ShopAPI.Tests;

/// <summary>Unit tests for the HATEOAS LinkService.</summary>
public class LinkServiceTests
{
    private readonly LinkService _sut;

    /// <summary>Sets up a mocked HTTP context with a known base URL.</summary>
    public LinkServiceTests()
    {
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Scheme).Returns("http");
        mockRequest.Setup(r => r.Host).Returns(new HostString("localhost", 5000));

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        _sut = new LinkService(mockAccessor.Object);
    }

    /// <summary>Product links should contain self, update, delete, and all.</summary>
    [Fact]
    public void GetProductLinks_ReturnsExpectedKeys()
    {
        var links = _sut.GetProductLinks(1);

        links.Should().ContainKeys("self", "update", "delete", "all");
        links["self"].Should().Be("http://localhost:5000/api/products/1");
    }

    /// <summary>Category links should include a products link with query param.</summary>
    [Fact]
    public void GetCategoryLinks_ReturnsProductsLink()
    {
        var links = _sut.GetCategoryLinks(3);

        links["products"].Should().Be("http://localhost:5000/api/products?categoryId=3");
    }

    /// <summary>Order links should include an update-status link.</summary>
    [Fact]
    public void GetOrderLinks_ReturnsStatusLink()
    {
        var links = _sut.GetOrderLinks(5);

        links["update-status"].Should().Be("http://localhost:5000/api/orders/5/status");
    }
}
