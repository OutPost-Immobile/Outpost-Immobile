using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Interfaces;

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
    private readonly IParcelRepository _parcelRepository;
    private readonly IMailService _mailService;
    private readonly IUserRepository _userRepository;
    private readonly IStaticEnumHelper _staticEnumHelper;
    
    public BulkUpdateParcelStatusCommandHandler(IParcelRepository parcelRepository, IMailService mailService, IUserRepository userRepository, IStaticEnumHelper staticEnumHelper)
    {
        _parcelRepository = parcelRepository;
        _mailService = mailService;
        _userRepository = userRepository;
        _staticEnumHelper = staticEnumHelper;
    }
    
    public async Task Handle(BulkUpdateParcelStatusCommand command, CancellationToken cancellationToken)
    {
        var userDataDict = new Dictionary<string, (string, string)>();
        
        var parcelReceivers = await _parcelRepository.GetReceiverIdsFromParcels(command.ParcelModels.Select(x => x.FriendlyId));

        var existingParcelReceivers = parcelReceivers
            .Where(x => x.Item2 != null)
            .Select(x => (x.Item1, (Guid)x.Item2!));

        var parcelStatusTranslations =
            await _staticEnumHelper.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl);

        foreach (var (parcelId, userId) in existingParcelReceivers)
        {
            var recipientData = await _userRepository.GetUserEmailCredentials([userId]);

            var (email, name) = recipientData.First();
            
            userDataDict.Add(parcelId, (email, name));
        }
        
        foreach (var parcelModel in command.ParcelModels)
        {
            var (email, name) = userDataDict[parcelModel.FriendlyId];
            
            var request = new SendEmailRequest
            {
                RecipientMailAddress = email,
                RecipientName = name,
                MailSubject = "Zmiana statusu",
                MailBody = $"Status paczki: {parcelModel.FriendlyId} został zmieniony na: {parcelStatusTranslations[(int)parcelModel.Status]}"
            };   
            
            if (parcelModel.Status == ParcelStatus.Forgotten)
            {
                request = new SendEmailRequest
                {
                    RecipientMailAddress = "adresKierownika@kierownik.com",
                    RecipientName = "Kierownik",
                    MailSubject = "Zmiana statusu",
                    MailBody = $"Status paczki: {parcelModel.FriendlyId} został zmieniony na: {parcelStatusTranslations[(int)parcelModel.Status]}"
                };   
            }
            
            await _mailService.SendMessage(request);

            await _parcelRepository.UpdateParcelStatusAsync(parcelModel.FriendlyId, parcelModel.Status);
        }
    }
}