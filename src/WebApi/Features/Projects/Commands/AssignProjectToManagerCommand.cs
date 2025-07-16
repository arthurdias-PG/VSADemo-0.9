using MediatR;
using System;
using System.Text.Json.Serialization;
using VSADemo.Common.Domain.Managers;
using VSADemo.Common.Domain.Projects;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Projects.Commands;

public static class AssignProjectToManagerCommand
{
  public record Request() : IRequest<ErrorOr<Guid>>
  {
    [JsonIgnore]
    public string? ProjectId { get; set; }
    public string? ManagerId { get; set; }
  }

  public class Endpoint : IEndpoint
  {
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
      endpoints
        .MapApiGroup(ProjectsFeature.FeatureName)
        .MapPost("/{projectId}/assign-manager",
          async (ISender sender, string projectId, Request request, CancellationToken cancellationToken) =>
          {
            request.ProjectId = projectId;
            var result = await sender.Send(request, cancellationToken);

            return result.Match(id => TypedResults.Ok(id), CustomResult.Problem);
          })
        .WithName("AssignProjectToManager")
        .ProducesPost();
    }
  }

  internal sealed class Validator : AbstractValidator<Request>
  {
    public Validator()
    {
      RuleFor(v => v.ProjectId)
        .NotEmpty()
        .NotNull()
        .WithMessage("Project ID must not be empty.")
        .Must(projectId => Guid.TryParse(projectId, out _))
        .WithMessage("Project ID must be a valid GUID.");

      RuleFor(v => v.ManagerId)
        .NotEmpty()
        .NotNull()
        .WithMessage("Manager ID must not be empty.")
        .Must(managerId => Guid.TryParse(managerId, out _))
        .WithMessage("Manager ID must be a valid GUID.");
    }
  }

  internal sealed class Handler(ApplicationDbContext dbContext)
      : IRequestHandler<Request, ErrorOr<Guid>>
  {
    public async Task<ErrorOr<Guid>> Handle(Request request, CancellationToken cancellationToken)
    {
      var projectId = ProjectId.From(Guid.Parse(request.ProjectId!));
      var project = await dbContext.Projects
          .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

      if (project is null)
        return ProjectErrors.NotFound;

      var managerId = ManagerId.From(Guid.Parse(request.ManagerId!));
      project.AssignManager(managerId);

      dbContext.Projects.Update(project);

      await dbContext.SaveChangesAsync(cancellationToken);

      return project.Id.Value;
    }
  }
}
