using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Parcels.Queries;

public record UpdateParcelStatusCommand : IRequest<UpdateParcelStatusCommand, Task>
{
    public required string Id { get; init; }
    public required ParcelStatus Status { get; init; }
}

internal class UpdateParcelStatusCommandHandler : IRequestHandler<UpdateParcelStatusCommand, Task>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IMailService _mailService;
    
    public UpdateParcelStatusCommandHandler(IParcelRepository parcelRepository, IMailService mailService)
    {
        _parcelRepository = parcelRepository;
        _mailService = mailService;
    }
    
    public async Task Handle(UpdateParcelStatusCommand command, CancellationToken cancellationToken)
    {
        await _parcelRepository.UpdateParcelStatusAsync(command.Id, command.Status);
    }
}