using System.Net;
using ECommerceBackend.Utils.Jwt;
using ECommerceBackend.Utils.Microservices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using StoreService.Application.Services;

namespace StoreService.Application.Tests.Unit;

public class OrderDetailsServiceTest
{
    [Fact]
    public async Task FetchAsync_ShouldProperlyDeserializeTheHttpResponse()
    {
        HttpResponseMessage httpResponseMessage = new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"{
                ""OrderEntityId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                ""UserId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa7"",
                ""OrderProducts"": [
                    {
                        ""OrderId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                        ""ProductId"": 1,
                        ""StoreLocationId"": 5,
                        ""ProductName"": ""Test Product"",
                        ""Quantity"": 5
                    }
                ],
                ""CreatedAt"": ""2024-01-29T10:00:00Z""
            }",
            System.Text.Encoding.UTF8,
            "application/json")
        };

        Mock<HttpMessageHandler> mockHttpMessageHandler = new();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        HttpClient httpClient = new(mockHttpMessageHandler.Object);

        OrderDetailsService service = new(new Mock<ILogger<OrderDetailsService>>().Object ,httpClient,
            Options.Create(new MicroserviceHosts
            {
                OrderServiceUrl = "http://placeholder",
                PaymentServiceUrl = "http://placeholder",
                ProductServiceUrl = "http://placeholder",
                StoreServiceUrl = "http://placeholder",
                UserServiceUrl = "http://placeholder"
            }), new JwtTokenReader(new InternalJwtTokenContainer()));

        var result = await service.FetchAsync("", "", CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Single(result.OrderProducts);
    }
}
