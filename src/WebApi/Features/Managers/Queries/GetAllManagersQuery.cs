using MediatR;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Managers.Queries;

public static class GetAllManagersQuery
{
    public record ManagerDto(Guid Id, string Name);

    public record Request : IRequest<ErrorOr<IReadOnlyList<ManagerDto>>>;

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ManagersFeature.FeatureName)
                .MapGet("/",
                    async (ISender sender, CancellationToken cancellationToken) =>
                    {
                        var request = new Request();
                        var result = await sender.Send(request, cancellationToken);
                        return result.Match(TypedResults.Ok, CustomResult.Problem);
                    })
                .WithName("GetAllManagers")
                .ProducesGet<IReadOnlyList<ManagerDto>>();
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator() { }
    }

    internal sealed class Handler : IRequestHandler<Request, ErrorOr<IReadOnlyList<ManagerDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<IReadOnlyList<ManagerDto>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Managers
                .Select(h => new ManagerDto(h.Id.Value, h.Name))
                .ToListAsync(cancellationToken);
        }
    }
}