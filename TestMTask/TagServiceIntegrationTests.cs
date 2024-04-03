using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using MTask.Data;
using MTask.Services;
using MTask.Data.Entities;

[TestFixture]
public class TagServiceTests
{
    private TagDbContext _dbContext;
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private TagService _tagService;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TagDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryTagDb")
            .Options;
        _dbContext = new TagDbContext(options);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"items\": [{\"name\": \"TestTag1\", \"count\": 10},{\"name\": \"TestTag2\", \"count\": 30}]}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.stackexchange.com")
        };
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _tagService = new TagService(_dbContext, _mockHttpClientFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task FetchTagsFromApiAndSaveAsync_FetchesTags()
    {
        // Act
        var tags = await _tagService.FetchTagsFromApiAndSaveAsync();

        // Assert
        Assert.That(tags, Has.Count.EqualTo(22)); 
    }

    [Test]
    public async Task ProcessTagsAsync_AddsTagsWithCorrectPercentages()
    {
        // Arrange
        var tags = new List<Tag>
    {
        new Tag { Name = "Tag1", Count = 10 },
        new Tag { Name = "Tag2", Count = 20 },
        new Tag { Name = "Tah3", Count = 30 }
    };
        decimal total = 60;

        // Act
        await _tagService.ProcessTagsAsync(tags);
        var savedTags = _dbContext.Tags.ToList();

        // Assert
        Assert.That(savedTags.Count, Is.EqualTo(3), "Liczba zapisanych tagów powinna wynosić 3.");
        Assert.That(savedTags[0].PercentageInWholePopulation, Is.EqualTo(Math.Round(10 / total * 100, 2)), "Niepoprawny procent dla Tag1.");
        Assert.That(savedTags[1].PercentageInWholePopulation, Is.EqualTo(Math.Round(20 / total * 100, 2)), "Niepoprawny procent dla Tag2.");
        Assert.That(savedTags[2].PercentageInWholePopulation, Is.EqualTo(Math.Round(30 / total * 100, 2)), "Niepoprawny procent dla Tah3.");
    }

    [Test]
    public async Task GetSortedAndPagedTags_ReturnsCorrectlySortedAndPagedResults()
    {
        //Arrange
        _dbContext.Tags.AddRange(new List<Tag>
    {
        new Tag { Name = "BTag", Count = 10, PercentageInWholePopulation = 10 },
        new Tag { Name = "ATag", Count = 5, PercentageInWholePopulation = 5 },
        new Tag { Name = "CTag", Count = 20, PercentageInWholePopulation = 20 }
    });
        await _dbContext.SaveChangesAsync();

        int pageNumber = 1;
        int pageSize = 2;
        string sortBy = "name";
        bool sortAscending = true;

        // Act
        var result = await _tagService.GetSortedAndPagedTags(pageNumber, pageSize, sortBy, sortAscending);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2), "Liczba zwróconych tagów powinna wynosić 2.");
        Assert.That(result[0].Name, Is.EqualTo("ATag"), "Pierwszy tag powinien być 'ATag'.");
        Assert.That(result[1].Name, Is.EqualTo("BTag"), "Drugi tag powinien być 'BTag'.");
    }
}