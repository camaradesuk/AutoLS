using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SyRF.LiteratureSearch.Endpoint.DTOs;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Services;
using SyRF.SharedKernel.Interfaces;
using Xunit;

namespace SyRF.LiteratureSearch.Tests
{
    public class PubmedServiceTest
    {
        [Fact]
        public async void ExecutePubmedQueryAndSaveRecords_Should()
        {
            //Arrange
            var mockWebClient = new Mock<IPubmedWebClient>();
            var mockFileService = new Mock<IFileService>();
            var mockUnitOfWork = new Mock<ILsUnitOfWork>();
            mockWebClient
                .Setup(x => x.SubmitSearch("mice"))
                .Returns(Task.FromResult(new PubmedResultQueryDto{WebEnv = "env", QueryKey = "query", Count = 700}));
            
            var sut = new PubmedService(mockWebClient.Object, mockFileService.Object, mockUnitOfWork.Object);
            //Act
            var result = sut.FindNewPubmedStudiesAndSave(Guid.Empty, Guid.Empty, "desctiption", Guid.Empty, "mice", 500);
            //Assert
            Assert.IsAssignableFrom<IAsyncEnumerable<PubmedXmlFileInfoDto>>(result);
        }
        
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