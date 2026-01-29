using OutpostImmobile.Core.Tests.Acceptance.FitNesse.Context;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;
using FitNesseTestContext = OutpostImmobile.Core.Tests.Acceptance.FitNesse.Context.TestContext;

namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Base fixture class for FitNesse tests providing common functionality
/// for test setup, teardown, and database access.
/// </summary>
public abstract class BaseFixture
{
    protected FitNesseTestContext Context => FitNesseTestContext.Instance;
    
    /// <summary>
    /// Reset the test context between tests
    /// </summary>
    public void ResetTestContext()
    {
        FitNesseTestContext.Reset();
    }
    
    /// <summary>
    /// Clear the list of sent emails
    /// </summary>
    public void ClearSentEmails()
    {
        Context.ClearSentEmails();
    }
    
    /// <summary>
    /// Get count of sent emails
    /// </summary>
    public int SentEmailsCount()
    {
        return Context.SentEmails.Count;
    }
    
    /// <summary>
    /// Check if email was sent to a specific address
    /// </summary>
    public bool EmailWasSentTo(string email)
    {
        return Context.SentEmails.Any(e => e.RecipientMailAddress == email);
    }
    
    /// <summary>
    /// Check if email contains specific text in body
    /// </summary>
    public bool EmailBodyContains(string text)
    {
        return Context.SentEmails.Any(e => e.MailBody.Contains(text));
    }
}
