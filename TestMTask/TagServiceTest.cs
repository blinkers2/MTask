using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using MTask.Data;
using MTask.Data.Entities;
using MTask.Services;
using System.Net;

namespace MTask.Tests
{
    [TestFixture]
    public class TagServiceTests
    {
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private TagDbContext _dbContext;
        private TagService _tagService;

        [SetUp]
        public void Setup()
        {
            // Mock HttpMessageHandler
            _mockMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://api.stackexchange.com/2.2/")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Setup InMemory DbContext
            var options = new DbContextOptionsBuilder<TagDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new TagDbContext(options);

            _tagService = new TagService(_dbContext, _mockHttpClientFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task FetchTagsFromApiAndSaveAsync_ReturnsCorrectTags()
        {
            // Arrange
            var fakeApiResponse = "{\"items\":[{\"name\":\"TestowyTag\",\"count\":666}]}";
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeApiResponse)
                });

            // Act
            var result = await _tagService.FetchTagsFromApiAndSaveAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(11));
            Assert.That(result[0].Name, Is.EqualTo("TestowyTag"));
            Assert.That(result[0].Count, Is.EqualTo(666));
        }

        [Test]
        public async Task FetchTagsFromApiAndSaveAsync_ReturnsEmptyListWhenApiResponseIsEmpty()
        {
            // Arrange
            var fakeApiResponse = "{}";
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeApiResponse)
                });

            // Act
            var result = await _tagService.FetchTagsFromApiAndSaveAsync();

            // Assert
            Assert.That(result, Is.Empty); 
        }

        [Test]
        public async Task ProcessTagsAsync_CalculatesPercentage()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Name = "TestowyTag", Count = 200 },
                new Tag { Name = "Java", Count = 800 }
            };

            // Act
            await _tagService.ProcessTagsAsync(tags);

            // Assert
            Assert.That(tags[0].PercentageInWholePopulation, Is.EqualTo(20));
            Assert.That(tags[1].PercentageInWholePopulation, Is.EqualTo(80));
        }

        [Test]
        public async Task ProcessTagsAsync_CalculatesPercentageCorrectlyWithFloat()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Name = "TestowyTag", Count = 100 },
                new Tag { Name = "Java", Count = 200 }
            };

            // Act
            await _tagService.ProcessTagsAsync(tags);

            // Assert
            Assert.That(tags[0].PercentageInWholePopulation, Is.EqualTo(33.33M).Within(0.01M));
            Assert.That(tags[1].PercentageInWholePopulation, Is.EqualTo(66.66M).Within(0.01M));
        }
    }
}