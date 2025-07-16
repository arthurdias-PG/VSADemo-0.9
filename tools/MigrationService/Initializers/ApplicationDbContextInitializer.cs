using Bogus;
using Microsoft.EntityFrameworkCore;
using VSADemo.Common.Domain.Managers;
using VSADemo.Common.Domain.Projects;
using VSADemo.Common.Persistence;

namespace MigrationService.Initializers;

public class ApplicationDbContextInitializer(ApplicationDbContext dbContext) : DbContextInitializerBase<ApplicationDbContext>(dbContext)
{
    private const int NumProjects = 10;
    private const int NumManagers = 4;

    private readonly string[] _projectNames =
    [
        "Project Alpha",
        "Project Beta",
        "Project Gamma",
        "Project Delta",
        "Project Epsilon",
        "Project Zeta",
        "Project Eta",
        "Project Theta",
        "Project Iota",
        "Project Kappa",
        "Project Lambda",
    ];

    private readonly string[] _managerNames =
    [
        "Alice Johnson",
        "Bob Smith",
        "Charlie Brown",
        "Diana Prince",
        "Ethan Hunt",
        "Fiona Gallagher",
        "George Costanza",
        "Hannah Montana",
        "Ian Malcolm",
        "Jane Doe"
    ];

    private readonly string[] _managerEmails =
    [
        "alice.johnson@example.com",
        "bob.smith@example.com",
        "charlie.brown@example.com",
        "diana.prince@example.com",
        "ethan.hunt@example.com",
        "fiona.gallagher@example.com",
        "george.costanza@example.com",
        "hannah.montana@example.com",
        "ian.malcolm@example.com",
        "jane.doe@example.com"
    ];

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var strategy = DbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
            var projects = await SeedProjectsAsync(cancellationToken);
            var managers = await SeedManagersAsync(cancellationToken);
            await AssignProjectsToManagerAsync(projects, managers, cancellationToken);

            // await DbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task<List<Project>> SeedProjectsAsync(CancellationToken cancellationToken)
    {
        if (DbContext.Projects.Any())
        {
            return new List<Project>();
        }

        var faker = new Faker<Project>()
            .CustomInstantiator(f =>
            {
                var name = f.PickRandom(_projectNames);
                // Set a random deadline between a week from now and 3 months from now
                var numOfDays = f.Random.Int(7, 90);
                var deadline = DateTime.UtcNow.AddDays(numOfDays);
                var project = Project.Create(name, deadline);
                return project;
            });

        var projects = faker.Generate(NumProjects);
        await DbContext.Projects.AddRangeAsync(projects, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return projects;
    }

    private async Task<List<Manager>> SeedManagersAsync(CancellationToken cancellationToken)
    {
        if (DbContext.Managers.Any())
        {
            return new List<Manager>();
        }

        var faker = new Faker<Manager>()
            .CustomInstantiator(f =>
                Manager.Create(
                    f.PickRandom(_managerNames),
                    f.PickRandom(_managerEmails)));

        var managers = faker.Generate(NumManagers).ToList();

        await DbContext.Managers.AddRangeAsync(managers, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return managers;
    }

    private async Task AssignProjectsToManagerAsync(List<Project> projects, List<Manager> managers, CancellationToken cancellationToken)
    {
        // Randomly assign each project to a manager using Project.AssignManager
        var faker = new Faker();
        foreach (var project in projects)
        {
            var manager = faker.PickRandom(managers);
            project.AssignManager(manager.Id);
            DbContext.Projects.Update(project);
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}