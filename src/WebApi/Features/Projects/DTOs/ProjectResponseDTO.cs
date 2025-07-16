using VSADemo.Common.Domain.Projects;

namespace VSADemo.Features.Projects.DTOs;

public record ProjectResponseDto(Guid Id, string Name, string? ManagerName)
{
  public static ProjectResponseDto FromDomain(Project project) =>
      new(project.Id.Value, project.Name, project.Manager?.Name);
}

