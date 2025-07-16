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
}