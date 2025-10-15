using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaperlessREST.API.Controllers;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Application.Commands;
using PaperlessREST.DataAccess.Service;
using PaperlessREST.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace PaperlessREST.Tests.Controllers
{
    [TestFixture]
    public class MetaDataControllerTests
    {
        private Mock<IMetaDataService> _metaDataServiceMock;
        private Mock<RabbitMqService> _rabbitMqServiceMock;
        private Mock<ILogger<MetaDataController>> _loggerMock;
        private MetaDataController _controller;

        [TearDown]
        public void TearDown() // necessary otherwise error because dispose no teardown something error
        {
            _controller?.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _metaDataServiceMock = new Mock<IMetaDataService>();
            //_rabbitMqServiceMock = new Mock<RabbitMqService>();
            _rabbitMqServiceMock = null;
            _loggerMock = new Mock<ILogger<MetaDataController>>();

            _controller = new MetaDataController(
                _metaDataServiceMock.Object,
                //_rabbitMqServiceMock.Object,
                null!,
                _loggerMock.Object
            );
        }

        [Test]
        public void GetMetaDatas_ReturnsOk_WithList()
        {
            var fakeList = new List<MetaData> // a list with only one document only for this test
            {
                new MetaData(Guid.NewGuid(), "Doc1", "pdf", 1234, "Summary", DateTime.Now, DateTime.Now)
            };

            _metaDataServiceMock.Setup(s => s.GetAllMetaData()).Returns(fakeList); // configures _metaDataServiceMock to return the list when GetAllMetaData is called

            var result = _controller.GetMetaDatas(); // calls the function and gets that list with that one document

            var okResult = result.Result as OkObjectResult; // converts a wrapper (Ok, NoContent, BadRequest) into a value and checks if the reulst was really Ok
            Assert.NotNull(okResult);                       // if result is null than the test fails
            var value = okResult.Value as List<MetaData>;   // gets the content (the list with only one document) from the Ok response
            Assert.That(value, Is.Not.Null);                // if the list is null the test fails
            Assert.That(value.Count, Is.EqualTo(1));        // if there is not 1 document the test fails
        }

        [Test]
        public void MetaDataAsync_ReturnsNoContent_WhenNotFound()
        {
            _metaDataServiceMock.Setup(s => s.GetMetaDataByGuid(It.IsAny<Guid>())).Returns((MetaData?)null);

            var result = _controller.MetaDataAsync(Guid.NewGuid());

            Assert.IsInstanceOf<NoContentResult>(result.Result);
        }

        [Test]
        public void DeleteMetaData_ReturnsBadRequest_WhenGuidIsEmpty()
        {
            var result = _controller.DeleteMetaData(Guid.Empty); 

            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Invalid GUID."));
        }

        [Test]
        public async Task UploadDocument_ReturnsBadRequest_WhenFileIsNull()
        {
            var result = await _controller.UploadDocument(null);

            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("No file provided."));
        }

        [Test]
        public void UpdateMetaData_ReturnsBadRequest_WhenIdsMismatch()
        {
            var metadata = new MetaData(Guid.NewGuid(), "Title", "pdf", 2000, "summary", DateTime.Now, DateTime.Now);

            var result = _controller.UpdateMetaData(Guid.NewGuid(), metadata);

            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Document ID mismatch."));
        }
    }
}
