using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Core.Paralizator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Parcels.Commands;

public record UpdateParcelStatusCommand : IRequest<UpdateParcelStatusCommand, bool>
{
    public required Guid ParcelId { get; init; }
    public required ParcelStatus Status { get; init; }
}

internal class UpdateParcelStatusCommandHandler : IRequestHandler<UpdateParcelStatusCommand, bool>
{
    private readonly IMailService _mailService;

    public UpdateParcelStatusCommandHandler(IMailService mailService)
    {
        _mailService = mailService;
    }

    public Task<bool> Handle(UpdateParcelStatusCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}