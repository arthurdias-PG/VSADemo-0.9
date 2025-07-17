using System;
using VSADemo.Common.Domain.Base;
using VSADemo.Common.Domain.Managers;

namespace VSADemo.Common.Domain.Projects;

// Ensure stongly typed IDs are registered in 'VogenEfCoreConverters'
// For strongly typed IDs, check out the rule: https://www.ssw.com.au/rules/do-you-use-strongly-typed-ids/
[ValueObject<Guid>]
public readonly partial struct ProjectId;

public class Project : AggregateRoot<ProjectId>
{
    public const int NameMaxLength = 100;

    private string _name = null!;
    private DateTime _deadline = DateTime.MaxValue;

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

    public DateTime Deadline
    {
        get => _deadline;
        set
        {
            ThrowIfLessThanOrEqual(value, DateTime.UtcNow, nameof(Deadline));
            ThrowIfEqual(value, DateTime.MaxValue, nameof(Deadline));

            _deadline = value;
        }
    }

    public ManagerId? ManagerId { get; private set; }
    public Manager? Manager { get; private set; }

    private Project() { } // Needed for EF Core

    public static Project Create(string name, DateTime deadline)
    {
        var project = new Project { Id = ProjectId.From(Guid.CreateVersion7()), Name = name, Deadline = deadline };

        return project;
    }

    public void AssignManager(ManagerId managerId)
    {
        if (Manager is not null)
        {
            throw new InvalidOperationException("Project already has an assigned manager.");
        }


        if (DateTime.UtcNow > Deadline)
        {
            throw new InvalidOperationException("Cannot assign a manager to a project with a past deadline.");
        }

        ManagerId = managerId;
        AddDomainEvent(new ProjectAssignedToManagerEvent(this, managerId));
    }
}