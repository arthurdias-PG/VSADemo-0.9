using MediatR;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Projects.Queries;

public static class GetAllProjectsQuery
{
    public record ProjectDto(Guid Id, string Name);

    public record Request : IRequest<ErrorOr<IReadOnlyList<ProjectDto>>>;

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ProjectsFeature.FeatureName)
                .MapGet("/",
                    async (ISender sender, CancellationToken cancellationToken) =>
                    {
                        var request = new Request();
                        var result = await sender.Send(request, cancellationToken);
                        return result.Match(TypedResults.Ok, CustomResult.Problem);
                    })
                .WithName("GetAllProjects")
                .ProducesGet<IReadOnlyList<ProjectDto>>();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator() { }
    }

    internal sealed class Handler : IRequestHandler<Request, ErrorOr<IReadOnlyList<ProjectDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<IReadOnlyList<ProjectDto>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Projects
                .Select(h => new ProjectDto(h.Id.Value, h.Name))
                .ToListAsync(cancellationToken);
        }
    }
}