using DispatchR.Abstractions.Send;

namespace OutpostImmobile.Core.Maczkopats.Commands;

public record MaczkopatAddLogCommand : IRequest<MaczkopatAddLogCommand, Task>
{
    
}

internal class MaczkopatAddLogCommandHandler : IRequestHandler<MaczkopatAddLogCommand, Task>
{
    public Task Handle(MaczkopatAddLogCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}