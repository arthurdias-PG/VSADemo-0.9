using Microsoft.EntityFrameworkCore;
using VSADemo.Common.Domain.Teams;
using VSADemo.Features.Teams.Commands;
using VSADemo.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;

namespace VSADemo.IntegrationTests.Endpoints.Teams.Commands;

public class CreateTeamCommandTests(TestingDatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task Command_ShouldCreateTeam()
    {
        // Arrange
        var cmd = new CreateTeamCommand.Request("Clark Kent");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PostAsJsonAsync("/api/teams", cmd, CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        var item = await GetQueryable<Team>().FirstAsync(CancellationToken);

        item.Should().NotBeNull();
        item.Name.Should().Be(cmd.Name);
        item.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    }
}