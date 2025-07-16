using MediatR;
using VSADemo.Common.Domain.Projects;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Projects.Commands;

public static class CreateProjectCommand
{
    public record Request(string Name, DateTime Deadline) : IRequest<ErrorOr<Guid>>;

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ProjectsFeature.FeatureName)
                .MapPost("/",
                    async (ISender sender, Request request, CancellationToken ct) =>
                    {
                        var result = await sender.Send(request, ct);
                        return result.Match(id => TypedResults.Created($"/projects/{id}", id), CustomResult.Problem);
                    })
                .WithName("CreateProject")
                .ProducesPost();
        }
    }

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Name)
                .NotEmpty()
                .MaximumLength(Project.NameMaxLength)
                .WithMessage("Name must not be empty and not exceed 100 characters.");

            RuleFor(v => v.Deadline)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Deadline must be a future date and time.");
        }
    }

    internal sealed class Handler(ApplicationDbContext dbContext)
        : IRequestHandler<Request, ErrorOr<Guid>>
    {
        public async Task<ErrorOr<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            var project = Project.Create(request.Name, request.Deadline);

            await dbContext.Projects.AddAsync(project, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return project.Id.Value;
        }
    }
}