namespace PraticeManagement
{
    using System.Web.Security;
    using PracticeManagementService;

    /// <summary>
    /// Custom Membership provider to override reset password functionality.
    /// </summary>
    public sealed class CustomMembershipProvider : SqlMembershipProvider
    {
        public override string GeneratePassword()
        {
            string password = base.GeneratePassword();
            return PasswordGeneratorUtility.GeneratePassword(password);
        }
    }
}

