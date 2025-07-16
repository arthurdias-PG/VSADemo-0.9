using MediatR;
using System;
using VSADemo.Common.Domain.Projects;
using VSADemo.Features.Projects.DTOs;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Projects.Queries;

public class GetProjectQuery
{
  public record Request(string Id) : IRequest<ErrorOr<ProjectResponseDto>>;

  public class Endpoint : IEndpoint
  {
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
      endpoints
          .MapApiGroup(ProjectsFeature.FeatureName)
          .MapGet("/{id}",
              async (ISender sender, string id, CancellationToken cancellationToken) =>
              {
                var request = new Request(id);
                var result = await sender.Send(request, cancellationToken);
                return result.Match(TypedResults.Ok, CustomResult.Problem);
              })
          .WithName("GetProject")
          .ProducesGet<ProjectResponseDto>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .ProducesProblem(StatusCodes.Status404NotFound)
          .ProducesProblem(StatusCodes.Status500InternalServerError)
          .WithSummary("Get a project by ID")
          .WithDescription("Retrieves a project by its unique identifier.")
          .WithTags(ProjectsFeature.FeatureName);
    }
  }

  public class Validator : AbstractValidator<Request>
  {
    public Validator()
    {
      RuleFor(v => v.Id)
          .Cascade(CascadeMode.Stop)
          .NotNull()
          .WithMessage("Project ID must not be null.")
          .NotEmpty()
          .WithMessage("Project ID must not be an empty string.")
          .Must(id => Guid.TryParse(id, out _))
          .WithMessage("Project ID must be a valid GUID.");
    }
  }

  internal sealed class Handler : IRequestHandler<Request, ErrorOr<ProjectResponseDto>>
  {
    private readonly ApplicationDbContext _dbContext;

    public Handler(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectResponseDto>> Handle(Request request, CancellationToken cancellationToken)
    {
      var projectId = ProjectId.From(Guid.Parse(request.Id));

      var project = await _dbContext.Projects.Where(p => p.Id == projectId)
          .AsNoTracking()
          .Include(p => p.Manager)
          .FirstOrDefaultAsync(cancellationToken);

      if (project is null)
      {
        return ProjectErrors.NotFound;
      }

      return ProjectResponseDto.FromDomain(project);
    }
  }
}
