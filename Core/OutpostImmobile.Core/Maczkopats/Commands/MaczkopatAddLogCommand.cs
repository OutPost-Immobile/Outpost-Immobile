
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Maczkopats.Commands;

public record MaczkopatAddLogCommand : IRequest<MaczkopatAddLogCommand, bool>
{
    public required MaczkopatEventLogType LogType { get; init; }
}

internal class MaczkopatAddLogCommandHandler : IRequestHandler<MaczkopatAddLogCommand, bool>
{
    public Task<bool> Handle(MaczkopatAddLogCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}