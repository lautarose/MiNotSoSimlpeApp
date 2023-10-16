using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Moq.Protected;

namespace MiNotSoSimpleAppTests
{
    public class ApiServiceTests
    {
        [Test]
        public async Task GetMyModelsAsync_ReturnsDataFromHttpClient()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
               Content = new StringContent("[{ \"UserId\": 1, \"Id\": 1, \"Title\": \"Test Title 1\", \"Body\": \"Test Body 1\" }, " +
                                 "{ \"UserId\": 2, \"Id\": 2, \"Title\": \"Test Title 2\", \"Body\": \"Test Body 2\" }, " +
                                 "{ \"UserId\": 3, \"Id\": 3, \"Title\": \"Test Title 3\", \"Body\": \"Test Body 3\" }]")
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            serviceCollection.AddTransient<IApiService>(_ => new ApiService(new HttpClient(mockHttpMessageHandler.Object)));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var apiService = serviceProvider.GetRequiredService<IApiService>();

            // Act
            var result = await apiService.GetMyModelsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Test Title 1", result.FirstOrDefault().Title);
        }
    }
}