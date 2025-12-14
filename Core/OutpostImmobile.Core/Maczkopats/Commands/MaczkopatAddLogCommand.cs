using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Maczkopats.Commands;

public record MaczkopatAddLogCommand : IRequest<MaczkopatAddLogCommand, Task>
{
    public required Guid MaczkopatId { get; init; }
    public required MaczkopatEventLogType LogType { get; init; }
}

internal class MaczkopatAddLogCommandHandler : IRequestHandler<MaczkopatAddLogCommand, Task>
{
    private readonly IMaczkopatRepository _maczkopatRepository;

    public MaczkopatAddLogCommandHandler(IMaczkopatRepository maczkopatRepository)
    {
        _maczkopatRepository = maczkopatRepository;
    }

    public async Task Handle(MaczkopatAddLogCommand request, CancellationToken ct)
    {
        await _maczkopatRepository.AddLogAsync(request.MaczkopatId, request.LogType, ct);
    }
}