using DispatchR.Abstractions.Send;
using OutpostImmobile.Communication.Interfaces;

namespace OutpostImmobile.Core.Parcels.Commands;

public record UpdateParcelStatusCommand : IRequest<UpdateParcelStatusCommand, Task>
{
    public required Guid ParcelId { get; init; }
}

internal class UpdateParcelStatusCommandHandler : IRequestHandler<UpdateParcelStatusCommand, Task>
{
    private readonly IMailService _mailService;

    public UpdateParcelStatusCommandHandler(IMailService mailService)
    {
        _mailService = mailService;
    }

    public Task Handle(UpdateParcelStatusCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}