namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Fixture for authorization-related operations.
/// Used for testing login, logout, and role-based access control.
/// </summary>
public class AuthorizationFixture : BaseFixture
{
    // Properties for Decision Table input
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    
    /// <summary>
    /// Simulate login as a user with a specific role
    /// </summary>
    public bool Login()
    {
        if (string.IsNullOrEmpty(Role))
        {
            return false;
        }
        
        Context.IsAuthenticated = true;
        Context.CurrentUserRole = Role;
        
        return true;
    }
    
    /// <summary>
    /// Login with specific role (used in Script tables)
    /// </summary>
    public bool LoginWithRole(string role)
    {
        Context.IsAuthenticated = true;
        Context.CurrentUserRole = role;
        return true;
    }
    
    /// <summary>
    /// Logout the current user
    /// </summary>
    public void Logout()
    {
        Context.IsAuthenticated = false;
        Context.CurrentUserRole = null;
    }
    
    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        return Context.IsAuthenticated;
    }
    
    /// <summary>
    /// Get current user role
    /// </summary>
    public string? GetCurrentRole()
    {
        return Context.CurrentUserRole;
    }
    
    /// <summary>
    /// Check if current user has a specific role
    /// </summary>
    public bool HasRole(string role)
    {
        return Context.CurrentUserRole == role;
    }
    
    /// <summary>
    /// Check if user can access courier functions
    /// </summary>
    public bool CanAccessCourierFunctions()
    {
        return Context.IsAuthenticated && Context.CurrentUserRole == "Kurier";
    }
    
    /// <summary>
    /// Check if user can access admin functions
    /// </summary>
    public bool CanAccessAdminFunctions()
    {
        return Context.IsAuthenticated && Context.CurrentUserRole == "Administrator";
    }
    
    /// <summary>
    /// Attempt to perform an action as unauthorized user (should fail)
    /// </summary>
    public bool AttemptUnauthorizedAction()
    {
        // Return true if action is blocked (as expected)
        return !Context.IsAuthenticated || Context.CurrentUserRole != "Kurier";
    }
}
