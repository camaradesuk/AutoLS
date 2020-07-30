using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SyRF.LiteratureSearch.Endpoint.Services;
using Xunit;

namespace SyRF.LiteratureSearch.Tests
{
    public class PubmedWebClientTest
    {

        [Fact]
        public async void SubmitSearch_Should_Return_SearchProperties()
        {
            //Arrange
            var httpMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMock.Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{'esearchresult' : {'webenv': 'environmentNumber' ,'querykey':'1', 'count': '2'}}"),
                })
                .Verifiable();
            var httpClient = new HttpClient(httpMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var sut = new PubmedWebClient(httpClient);
            //Act
            var properties = await sut.SubmitSearch("mice");
            //Assert
            Assert.Equal("environmentNumber", properties.WebEnv);
            Assert.Equal("1", properties.QueryKey);
        }
    }
}  