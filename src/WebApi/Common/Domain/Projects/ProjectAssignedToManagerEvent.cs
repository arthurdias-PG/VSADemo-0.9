using System;
using VSADemo.Common.Domain.Base.EventualConsistency;
using VSADemo.Common.Domain.Base.Interfaces;
using VSADemo.Common.Domain.Managers;

namespace VSADemo.Common.Domain.Projects;

public record ProjectAssignedToManagerEvent(Project Project, ManagerId ManagerId, string ManagerName, Email ManagerEmail) : IDomainEvent
{
  public static readonly Error ManagerNotFound = EventualConsistencyError.From(
      code: "ProjectAssignedToManager.ManagerNotFound",
      description: "Manager not found");

  public static readonly Error ProjectNotFound = EventualConsistencyError.From(
      code: "ProjectAssignedToManager.ProjectNotFound",
      description: "Project not found");
}
