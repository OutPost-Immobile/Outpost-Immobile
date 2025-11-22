using OutpostImmobile.Persistence.Common.Helpers;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Common.Consts;

public class EnumKeys
{
    #region ParcelStatus
    
    public static string ParcelStatus_Expedited_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.Expedited, TranslationLanguage.Pl);
    public static string ParcelStatus_InTransit_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.InTransit, TranslationLanguage.Pl);
    public static string ParcelStatus_Delivered_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.Delivered, TranslationLanguage.Pl);
    public static string Parcel_InWarehouse_StatusPl => EnumKeyGenerator.GenerateKey(ParcelStatus.InWarehouse, TranslationLanguage.Pl);
    public static string ParcelStatus_Forgotten_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.Forgotten, TranslationLanguage.Pl);
    public static string ParcelStatus_Deleted_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.Deleted, TranslationLanguage.Pl);
    public static string ParcelStatus_Sent_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.Sent, TranslationLanguage.Pl);
    public static string ParcelStatus_ToReturn_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.ToReturn, TranslationLanguage.Pl);
    public static string ParcelStatus_SendToStorage_Pl => EnumKeyGenerator.GenerateKey(ParcelStatus.SendToStorage, TranslationLanguage.Pl);

    #endregion

    #region PayloadSize

    public static string PayloadSize_Small_Pl => EnumKeyGenerator.GenerateKey(PayloadSize.Small, TranslationLanguage.Pl);
    public static string PayloadSize_Medium_Pl = EnumKeyGenerator.GenerateKey(PayloadSize.Medium, TranslationLanguage.Pl);
    public static string PayloadSize_Large_Pl = EnumKeyGenerator.GenerateKey(PayloadSize.Large, TranslationLanguage.Pl);
    public static string PayloadSize_XLarge_Pl = EnumKeyGenerator.GenerateKey(PayloadSize.XLarge, TranslationLanguage.Pl);

    #endregion

    #region MaczkopatEventLogType
    
    public static string MaczkopatEventLogType_LockerOpened_Pl => EnumKeyGenerator.GenerateKey(MaczkopatEventLogType.LockerOpened, TranslationLanguage.Pl);
    public static string MaczkopatEventLogType_LockerClosed_Pl => EnumKeyGenerator.GenerateKey(MaczkopatEventLogType.LockerClosed, TranslationLanguage.Pl);
    public static string MaczkopatEventLogType_OpenedByForce_Pl => EnumKeyGenerator.GenerateKey(MaczkopatEventLogType.OpenedByForce, TranslationLanguage.Pl);
    public static string MaczkopatEventLogType_Error_Pl => EnumKeyGenerator.GenerateKey(MaczkopatEventLogType.Error, TranslationLanguage.Pl);
    
    #endregion

    #region ParcelEventLogType

    public static string ParcelEventLogType_StatusChange_Pl => EnumKeyGenerator.GenerateKey(ParcelEventLogType.StatusChange, TranslationLanguage.Pl);

    #endregion
}