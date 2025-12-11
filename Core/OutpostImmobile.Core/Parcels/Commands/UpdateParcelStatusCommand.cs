using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Parcels.Queries;

public record UpdateParcelStatusCommand : IRequest<UpdateParcelStatusCommand, bool>
{
    public required Guid Id { get; init; }
    public required ParcelStatus Status { get; init; }
}

internal class UpdateParcelStatusCommandHandler : IRequestHandler<UpdateParcelStatusCommand, bool>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IMailService _mailService;
    
    public UpdateParcelStatusCommandHandler(IParcelRepository parcelRepository, IMailService mailService)
    {
        _parcelRepository = parcelRepository;
        _mailService = mailService;
    }
    
    public async Task<bool> Handle(UpdateParcelStatusCommand command, CancellationToken cancellationToken)
    {
        var success = await _parcelRepository.UpdateParcelStatusAsync(command.Id, command.Status);
        return success;
    }
}