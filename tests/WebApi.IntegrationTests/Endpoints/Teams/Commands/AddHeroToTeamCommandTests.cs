using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VSADemo.Common.Domain.Teams;
using VSADemo.Features.Teams.Commands;
using VSADemo.IntegrationTests.Common;
using VSADemo.IntegrationTests.Common.Factories;
using System.Net;
using System.Net.Http.Json;

namespace VSADemo.IntegrationTests.Endpoints.Teams.Commands;

public class AddHeroToTeamCommandTests(TestingDatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task Command_ShouldAddHeroToTeam()
    {
        // Arrange
        var hero = HeroFactory.Generate();
        var team = TeamFactory.Generate();
        await AddAsync(team);
        await AddAsync(hero);
        var teamId = team.Id.Value;
        var heroId = hero.Id.Value;
        var cmd = new AddHeroToTeamCommand.Request();
        var client = GetAnonymousClient();

        // Act
        var result = await client.PostAsJsonAsync($"/api/teams/{teamId}/heroes/{heroId}", cmd, CancellationToken);

        // Assert
        var updatedTeam = await GetQueryable<Team>()
            .WithSpecification(new TeamByIdSpec(team.Id))
            .FirstOrDefaultAsync(CancellationToken);

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        updatedTeam.Should().NotBeNull();
        updatedTeam!.Heroes.Should().HaveCount(1);
        updatedTeam.Heroes.First().Id.Should().Be(hero.Id);
        updatedTeam.TotalPowerLevel.Should().Be(hero.PowerLevel);
    }
}