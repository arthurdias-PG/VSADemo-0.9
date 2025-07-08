using VSADemo.Features.Teams.Queries;
using VSADemo.IntegrationTests.Common;
using VSADemo.IntegrationTests.Common.Factories;
using System.Net.Http.Json;

namespace VSADemo.IntegrationTests.Endpoints.Teams.Queries;

public class GetAllTeamsQueryTests(TestingDatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task Query_ShouldReturnAllTeams()
    {
        // Arrange
        const int entityCount = 10;
        var entities = TeamFactory.Generate(entityCount);
        await AddRangeAsync(entities);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GetFromJsonAsync<GetAllTeamsQuery.TeamDto[]>("/api/teams", CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Length.Should().Be(entityCount);

        var firstTeam = result.First();
        firstTeam.Id.Should().NotBeEmpty();
        firstTeam.Name.Should().NotBeEmpty();
    }
}