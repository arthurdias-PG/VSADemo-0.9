using MediatR;
using VSADemo.Common.Domain.Projects;
using VSADemo.Features.Projects.DTOs;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Projects.Queries;

public static class GetAllProjectsQuery
{
    public record Request : IRequest<ErrorOr<IReadOnlyList<ProjectResponseDto>>>;

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
                .ProducesGet<IReadOnlyList<ProjectResponseDto>>();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator() { }
    }

    internal sealed class Handler : IRequestHandler<Request, ErrorOr<IReadOnlyList<ProjectResponseDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<IReadOnlyList<ProjectResponseDto>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Projects.AsNoTracking().Include(p => p.Manager)
                .Select(p => ProjectResponseDto.FromDomain(p))
                .ToListAsync(cancellationToken);
        }
    }
}