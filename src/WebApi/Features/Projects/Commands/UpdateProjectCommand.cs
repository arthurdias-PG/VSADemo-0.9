using MediatR;
using VSADemo.Common.Domain.Projects;
using VSADemo.Host.Extensions;
using System.Text.Json.Serialization;

namespace VSADemo.Features.Projects.Commands;

public static class UpdateProjectCommand
{
    public record Request(string Name) : IRequest<ErrorOr<Guid>>
    {
        [JsonIgnore]
        public Guid ProjectId { get; set; }
    }

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ProjectsFeature.FeatureName)
                .MapPut("/{projectId:guid}",
                    async (ISender sender, Guid projectId, Request request, CancellationToken cancellationToken) =>
                    {
                        request.ProjectId = projectId;
                        var result = await sender.Send(request, cancellationToken);
                        return result.Match(_ => TypedResults.NoContent(), CustomResult.Problem);
                    })
                .WithName("UpdateProject")
                .ProducesPut();
        }
    }

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.ProjectId)
                .NotEmpty();

            RuleFor(v => v.Name)
                .NotEmpty();
        }
    }

    internal sealed class Handler(ApplicationDbContext dbContext)
        : IRequestHandler<Request, ErrorOr<Guid>>
    {
        public async Task<ErrorOr<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            var projectId = ProjectId.From(request.ProjectId);
            var project = await dbContext.Projects
                .FirstOrDefaultAsync(h => h.Id == projectId, cancellationToken);

            if (project is null)
                return ProjectErrors.NotFound;

            project.Name = request.Name;

            await dbContext.SaveChangesAsync(cancellationToken);

            return project.Id.Value;
        }
    }
}