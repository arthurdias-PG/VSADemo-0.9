using VSADemo.Common.Domain.Base;
using VSADemo.Common.Domain.Projects;

namespace VSADemo.Common.Domain.Managers;

// Ensure stongly typed IDs are registered in 'VogenEfCoreConverters'
// For strongly typed IDs, check out the rule: https://www.ssw.com.au/rules/do-you-use-strongly-typed-ids/
[ValueObject<Guid>]
public readonly partial struct ManagerId;

public class Manager : AggregateRoot<ManagerId>
{
    public const int NameMaxLength = 100;

    private string _name = null!;
    private Email _email = null!;

    private readonly List<Project> _projects = new();

    public string Name
    {
        get => _name;
        set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(Name));
            ThrowIfGreaterThan(value.Length, NameMaxLength, nameof(Name));
            _name = value;
        }
    }

    public Email Email
    {
        get => _email;
        set
        {
            ThrowIfNull(value, nameof(Email));
            _email = value;
        }
    }

    public IReadOnlyList<Project> Projects => _projects.AsReadOnly();

    private Manager() { } // Needed for EF Core

    public static Manager Create(string name, string emailAddress)
    {
        var email = new Email(emailAddress);
        var manager = new Manager { Id = ManagerId.From(Guid.CreateVersion7()), Name = name, Email = email };

        return manager;
    }

    public void AssignProjects(IEnumerable<Project> projects)
    {
        if (projects == null || !projects.Any())
        {
            throw new ArgumentException("Projects cannot be null or empty.", nameof(projects));
        }

        foreach (var project in projects)
        {
            if (_projects.Contains(project))
            {
                throw new InvalidOperationException($"Project {project.Id} is already assigned to this manager.");
            }
            _projects.Add(project);
            AddDomainEvent(new ProjectAssignedEvent(this, project.Id));
        }
    }

    public void RemoveProjects(IEnumerable<Project> projects)
    {
        if (projects == null || !projects.Any())
        {
            throw new ArgumentException("Projects cannot be null or empty.", nameof(projects));
        }

        foreach (var project in projects)
        {
            if (!_projects.Remove(project))
            {
                throw new InvalidOperationException($"Project {project.Id} is not assigned to this manager.");
            }
        }
    }
}