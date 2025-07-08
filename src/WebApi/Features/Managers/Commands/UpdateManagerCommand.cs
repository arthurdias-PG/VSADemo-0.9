using MediatR;
using VSADemo.Common.Domain.Managers;
using VSADemo.Host.Extensions;
using System.Text.Json.Serialization;

namespace VSADemo.Features.Managers.Commands;

public static class UpdateManagerCommand
{
    public record Request(string Name) : IRequest<ErrorOr<Guid>>
    {
        [JsonIgnore]
        public Guid ManagerId { get; set; }
    }

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ManagersFeature.FeatureName)
                .MapPut("/{managerId:guid}",
                    async (ISender sender, Guid managerId, Request request, CancellationToken cancellationToken) =>
                    {
                        request.ManagerId = managerId;
                        var result = await sender.Send(request, cancellationToken);
                        return result.Match(_ => TypedResults.NoContent(), CustomResult.Problem);
                    })
                .WithName("UpdateManager")
                .ProducesPut();
        }
    }

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.ManagerId)
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
            var managerId = ManagerId.From(request.ManagerId);
            var manager = await dbContext.Managers
                .FirstOrDefaultAsync(h => h.Id == managerId, cancellationToken);

            if (manager is null)
                return ManagerErrors.NotFound;

            manager.Name = request.Name;

            await dbContext.SaveChangesAsync(cancellationToken);

            return manager.Id.Value;
        }
    }
}