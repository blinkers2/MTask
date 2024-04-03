using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MTask.Controllers;
using MTask.Services;
using MTask.Data.Entities;

namespace MTask.Tests
{
    [TestFixture]
    public class TagControllerIntegrationTests
    {
        private Mock<ITagService> _mockTagService;
        private Mock<ILogger<TagController>> _mockLogger;
        private TagController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockTagService = new Mock<ITagService>();
            _mockLogger = new Mock<ILogger<TagController>>();
            _controller = new TagController(_mockLogger.Object, _mockTagService.Object);
        }

        [Test]
        public async Task FetchAndSaveTags_ReturnsOk_WithTags()
        {
            // Arrange
            var fakeTags = new List<Tag> { new Tag(), new Tag() };
            _mockTagService.Setup(s => s.FetchTagsFromApiAndSaveAsync()).ReturnsAsync(fakeTags);

            // Act
            var result = await _controller.FetchAndSaveTags();

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EquivalentTo(fakeTags));
        }

        [Test]
        public async Task FetchAndSaveTags_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockTagService.Setup(s => s.FetchTagsFromApiAndSaveAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.FetchAndSaveTags();

            // Assert
            Assert.That(result, Is.TypeOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult?.Value, Is.EqualTo("Internal server error"));
        }
    }
}
