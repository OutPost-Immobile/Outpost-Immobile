using DispatchR.Abstractions.Send;

namespace OutpostImmobile.Core.Parcels.Commands;

public record UpdateParcelStatusCommand : IRequest<UpdateParcelStatusCommand, Task>
{
    public required Guid ParcelId { get; init; }
}

internal class UpdateParcelStatusCommandHandler : IRequestHandler<UpdateParcelStatusCommand, Task>
{
    public Task Handle(UpdateParcelStatusCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}