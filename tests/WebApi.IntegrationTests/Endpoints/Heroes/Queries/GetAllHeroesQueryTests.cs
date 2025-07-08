using VSADemo.Features.Heroes.Queries;
using VSADemo.IntegrationTests.Common;
using VSADemo.IntegrationTests.Common.Factories;
using System.Net.Http.Json;

namespace VSADemo.IntegrationTests.Endpoints.Heroes.Queries;

public class GetAllHeroesQueryTests(TestingDatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task Query_ShouldReturnAllHeroes()
    {
        // Arrange
        const int entityCount = 10;
        var entities = HeroFactory.Generate(entityCount);
        await AddRangeAsync(entities);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GetFromJsonAsync<GetAllHeroesQuery.HeroDto[]>("/api/heroes", CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Length.Should().Be(entityCount);
    }
}