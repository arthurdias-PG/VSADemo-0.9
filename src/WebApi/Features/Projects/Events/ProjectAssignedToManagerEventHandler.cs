using MediatR;
using System;
using VSADemo.Common.Domain.Base.EventualConsistency;
using VSADemo.Common.Domain.Projects;

namespace VSADemo.Features.Projects.Events;

internal sealed class ProjectAssignedToManagerEventHandler(
    ApplicationDbContext dbContext,
    ILogger<ProjectAssignedToManagerEventHandler> logger) : INotificationHandler<ProjectAssignedToManagerEvent>
{
  public async Task Handle(ProjectAssignedToManagerEvent notification, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling project assigned to manager event for project {ProjectId}", notification.Project.Id);

    var project = await dbContext.Projects
        .FirstOrDefaultAsync(p => p.Id == notification.Project.Id, cancellationToken);

    if (project == null)
    {
      logger.LogError("Project with ID {ProjectId} not found", notification.Project.Id);
      throw new EventualConsistencyException(ProjectAssignedToManagerEvent.ProjectNotFound);
    }

    var manager = await dbContext.Managers
        .FirstOrDefaultAsync(m => m.Id == notification.ManagerId, cancellationToken);
    if (manager == null)
    {
      logger.LogError("Manager with ID {ManagerId} not found", notification.ManagerId);
      throw new EventualConsistencyException(ProjectAssignedToManagerEvent.ManagerNotFound);
    }

    // Send notification or perform additional actions here
    logger.LogInformation("Project {ProjectId} assigned to manager {ManagerName} ({ManagerEmail})",
        notification.Project.Id, manager.Name, manager.Email);
    // Example: Notify the manager via email or other means
    // await emailService.SendProjectAssignedNotification(manager.Email, notification.Project.Name);
  }
}