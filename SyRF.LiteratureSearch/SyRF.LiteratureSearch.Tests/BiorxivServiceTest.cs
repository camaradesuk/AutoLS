using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Moq;
using SyRF.LiteratureSearch.Endpoint.Interfaces;
using SyRF.LiteratureSearch.Endpoint.Model;
using SyRF.LiteratureSearch.Endpoint.Services;
using SyRF.SharedKernel.Interfaces;
using Xunit;

namespace SyRF.LiteratureSearch.Tests
{
    public class BiorxivServiceTest
    {
        public BiorxivServiceTest()
        {
        }
        

        [Fact]
        public void GetDeduplicatedStudies_Should_Return_List_Of_XElement()
        {
            //Arrange
            var httpClient = new HttpClient();
            var mockFileService = new Mock<IFileService>();
            var studyRef = new Collection<BiorxivStudyReference>();
            var mockUnitOfWork = new Mock<ILsUnitOfWork>();
            var sut = new BiorxivService(httpClient, mockFileService.Object,mockUnitOfWork.Object);
            //Act
            var result = sut.FindNewBiorxivStudiesAndSave("https://connect.biorxiv.org/relate/feed/181", Guid.Empty,
                Guid.Empty, Guid.Empty, "description", 200);
            //Assert
            Assert.NotNull(result);
        }
    }
}