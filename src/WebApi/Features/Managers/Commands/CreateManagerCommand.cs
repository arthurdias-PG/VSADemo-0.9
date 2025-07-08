using MediatR;
using VSADemo.Common.Domain.Managers;
using VSADemo.Host.Extensions;

namespace VSADemo.Features.Managers.Commands;

public static class CreateManagerCommand
{
    public record Request(string Name, string EmailAddress) : IRequest<ErrorOr<Guid>>;

    public class Endpoint : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapApiGroup(ManagersFeature.FeatureName)
                .MapPost("/",
                    async (ISender sender, Request request, CancellationToken ct) =>
                    {
                        var result = await sender.Send(request, ct);
                        return result.Match(_ => TypedResults.Created(), CustomResult.Problem);
                    })
                .WithName("CreateManager")
                .ProducesPost();
        }
    }

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(v => v.Name)
                .NotEmpty()
                .MaximumLength(Manager.NameMaxLength)
                .WithMessage("Name must not be empty and not exceed 100 characters.");

            RuleFor(v => v.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(Email.EmailMaxLength)
                .WithMessage("Email must be a valid email address and not exceed 255 characters.");
        }
    }

    internal sealed class Handler(ApplicationDbContext dbContext)
        : IRequestHandler<Request, ErrorOr<Guid>>
    {
        public async Task<ErrorOr<Guid>> Handle(Request request, CancellationToken cancellationToken)
        {
            var manager = Manager.Create(request.Name, request.EmailAddress);

            await dbContext.Managers.AddAsync(manager, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return manager.Id.Value;
        }
    }
}