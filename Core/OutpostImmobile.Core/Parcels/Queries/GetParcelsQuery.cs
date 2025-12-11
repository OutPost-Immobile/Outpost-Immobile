using OutpostImmobile.Core.Common.Response;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Core.Parcels.Queries;

public class GetParcelsQuery : IRequest<GetParcelsQuery, PagedResponse<ParcelDataGridDto>>;

//TODO