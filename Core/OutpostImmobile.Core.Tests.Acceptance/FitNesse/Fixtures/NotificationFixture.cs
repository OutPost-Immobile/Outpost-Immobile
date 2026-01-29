namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Fixture for notification-related verification.
/// Used for testing email notifications sent to customers.
/// </summary>
public class NotificationFixture : BaseFixture
{
    /// <summary>
    /// Get total count of sent emails
    /// </summary>
    public int GetSentEmailsCount()
    {
        return Context.SentEmails.Count;
    }
    
    /// <summary>
    /// Check if notification was sent to specific email
    /// </summary>
    public bool NotificationSentTo(string email)
    {
        return Context.SentEmails.Any(e => e.RecipientMailAddress == email);
    }
    
    /// <summary>
    /// Check if notification contains specific text
    /// </summary>
    public bool NotificationContains(string text)
    {
        return Context.SentEmails.Any(e => 
            e.MailBody.Contains(text) || e.MailSubject.Contains(text));
    }
    
    /// <summary>
    /// Check if notification about parcel status change was sent
    /// </summary>
    public bool StatusChangeNotificationSent(string parcelId)
    {
        return Context.SentEmails.Any(e => e.MailBody.Contains(parcelId));
    }
    
    /// <summary>
    /// Check if notification about "W maczkopacie" status was sent
    /// </summary>
    public bool InMaczkopatNotificationSent()
    {
        return Context.SentEmails.Any(e => e.MailBody.Contains("W maczkopacie"));
    }
    
    /// <summary>
    /// Check if notification about "Wysłana do magazynu" status was sent
    /// </summary>
    public bool SendToStorageNotificationSent()
    {
        return Context.SentEmails.Any(e => e.MailBody.Contains("Wysłana do magazynu"));
    }
    
    /// <summary>
    /// Check if notification about "W tranzycie" status was sent
    /// </summary>
    public bool InTransitNotificationSent()
    {
        return Context.SentEmails.Any(e => e.MailBody.Contains("W tranzycie"));
    }
    
    /// <summary>
    /// Get the last sent email recipient
    /// </summary>
    public string GetLastEmailRecipient()
    {
        return Context.SentEmails.LastOrDefault()?.RecipientMailAddress ?? "None";
    }
    
    /// <summary>
    /// Get the last sent email subject
    /// </summary>
    public string GetLastEmailSubject()
    {
        return Context.SentEmails.LastOrDefault()?.MailSubject ?? "None";
    }
    
    /// <summary>
    /// Clear all sent emails (for test isolation)
    /// </summary>
    public void ClearNotifications()
    {
        Context.ClearSentEmails();
    }
    
    /// <summary>
    /// Get count of notifications sent for a specific parcel
    /// </summary>
    public int GetNotificationsCountForParcel(string parcelId)
    {
        return Context.SentEmails.Count(e => e.MailBody.Contains(parcelId));
    }
}
