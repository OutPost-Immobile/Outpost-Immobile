using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Parcels.Queries;

public record TrackParcelByFriendlyIdQuery : IRequest<TrackParcelByFriendlyIdQuery, IEnumerable<ParcelLogDto>>
{
    public required string FriendlyId { get; init; }
}

internal class TrackParcelByFriendlyIdQueryHandler : IRequestHandler<TrackParcelByFriendlyIdQuery, IEnumerable<ParcelLogDto>>
{
    private readonly IParcelRepository _parcelRepository;

    public TrackParcelByFriendlyIdQueryHandler(IParcelRepository parcelRepository)
    {
        _parcelRepository = parcelRepository;
    }

    public async Task<IEnumerable<ParcelLogDto>> Handle(TrackParcelByFriendlyIdQuery query, CancellationToken cancellationToken)
    {
        var eventLogs = await _parcelRepository.GetParcelEventLogsAsync(query.FriendlyId);

        return eventLogs.Select(x => new ParcelLogDto
        {
            Message = x.Message,
            CreatedAt = x.CreatedAt,
            ParcelEventLogType = x.ParcelEventLogType.ToString(),
            ParcelId = x.ParcelId,
        });
    }

}
