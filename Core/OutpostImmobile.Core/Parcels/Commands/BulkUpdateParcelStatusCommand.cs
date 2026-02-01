using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.BusinessRules;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;

namespace OutpostImmobile.Core.Parcels.Commands;

public record BulkUpdateParcelStatusCommand : IRequest<BulkUpdateParcelStatusCommand, Task>
{
    public required IEnumerable<ParcelModel> ParcelModels { get; init; }

    public record ParcelModel
    {
        public required string FriendlyId { get; init; }
        public required ParcelStatus Status { get; init; }
    }
}

internal class BulkUpdateParcelStatusCommandHandler : IRequestHandler<BulkUpdateParcelStatusCommand, Task>
{
    private readonly IMailService _mailService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IStaticEnumHelper _staticEnumHelper;
    private readonly IEventLogFactory _eventLogFactory;
    private readonly IKMZBService _kmzbService;

    public BulkUpdateParcelStatusCommandHandler([FromKeyedServices("Parcel")] IEventLogFactory eventLogFactory, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory, IStaticEnumHelper staticEnumHelper, IMailService mailService, IKMZBService kmzbService)
    {
        _eventLogFactory = eventLogFactory;
        _dbContextFactory = dbContextFactory;
        _staticEnumHelper = staticEnumHelper;
        _mailService = mailService;
        _kmzbService = kmzbService;
    }
    
    public async Task Handle(BulkUpdateParcelStatusCommand request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var friendlyIds = request.ParcelModels.Select(x => x.FriendlyId);
        
        var parcelReceivers = await context.Parcels
            .Where(x => friendlyIds.Contains(x.FriendlyId))
            .Select(x => new
            {
                x.FriendlyId,
                x.ReceiverUserExternalId
            })
            .Where(x => x.ReceiverUserExternalId != null)
            .ToDictionaryAsync(x => x.FriendlyId,  x => x.ReceiverUserExternalId, cancellationToken: ct);

        var parcelStatusTranslations =
            await _staticEnumHelper.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl);

        var userIds = parcelReceivers.Select(x => x.Value);
        
        var usersInternal = await context.UsersInternal
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new UserModel(x.Id, x.Email, x.UserName))
            .ToListAsync(ct);
        
        var usersExternal = await context.UsersExternal
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new UserModel(x.Id, x.Email, x.Name))
            .ToListAsync(ct);

        var userModelList = usersInternal
                .Concat(usersExternal)
                .ToList();

        var parcelFriendlyIds = parcelReceivers.Select(x => x.Key);
        
        var parcelsToUpdate = await context.Parcels
            .Where(x => parcelFriendlyIds.Contains(x.FriendlyId))
            .Include(x => x.ParcelEventLogs)
            .ToListAsync(ct);
        
        foreach (var parcel in request.ParcelModels)
        {
            try
            {
                var user = userModelList.First(x => x.Id == parcelReceivers[parcel.FriendlyId]);

                var parcelToUpdate = parcelsToUpdate.First(x => x.FriendlyId == parcel.FriendlyId);
                
                await MaczkopatBusinessRules.ThrowIfCannotUpdateMaczkopatState(context, parcelToUpdate, parcel.Status);
                
                var logRequest = new CreateParcelEventLogTypeRequest
                {
                    ParcelId = parcelToUpdate.Id,
                    EventLog = ParcelEventLogType.StatusChange,
                    Message = $"Status zmieniony na: {parcelStatusTranslations[(int)parcel.Status]}"
                };

                var parcelEventLog = (ParcelEventLogEntity)(await _eventLogFactory.CreateEventLogAsync(logRequest, ct));
        
                parcelToUpdate.Status = parcel.Status;
                parcelToUpdate.ParcelEventLogs.Add(parcelEventLog);
                
                var mailRequest = new SendEmailRequest
                {
                    RecipientMailAddress = user.Email,
                    RecipientName = user.Username,
                    MailSubject = "Zmiana statusu",
                    MailBody = $"Status paczki: {parcel.FriendlyId} został zmieniony na: {parcelStatusTranslations[(int)parcel.Status]}"
                };   
            
                if (parcel.Status == ParcelStatus.Forgotten)
                {
                    mailRequest = new SendEmailRequest
                    {
                        RecipientMailAddress = "adresKierownika@kierownik.com",
                        RecipientName = "Kierownik",
                        MailSubject = "Zmiana statusu",
                        MailBody = $"Status paczki: {parcel.FriendlyId} został zmieniony na: {parcelStatusTranslations[(int)parcel.Status]}"
                    };   
                }
            
                await _mailService.SendMessage(mailRequest);
            }
            catch (MaczkopatStateException e)
            {
                await _kmzbService.CreateNewWarningAsync();
            }
        }
    }

    private record UserModel(Guid Id, string Email, string Username);
}