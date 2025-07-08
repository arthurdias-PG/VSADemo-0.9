using VSADemo.Common.Domain.Base.EventualConsistency;
using VSADemo.Common.Domain.Base.Interfaces;
using VSADemo.Common.Domain.Managers;
using VSADemo.Common.Domain.Projects;

public record ProjectAssignedEvent(Manager Manager, ProjectId ProjectId) : IDomainEvent
{
  public static readonly Error ProjectNotFound = EventualConsistencyError.From(code: "ProjectAssigned.ProjectNotFound", description: "The project was not found.");
}