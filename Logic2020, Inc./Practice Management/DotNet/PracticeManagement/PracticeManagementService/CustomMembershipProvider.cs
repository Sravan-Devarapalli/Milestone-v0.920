namespace PraticeManagement
{
    using System.Web.Security;
    using PracticeManagementService;
    using System.Web;
    using System;
    using System.Web.UI.MobileControls.Adapters;
    using System.Collections.Specialized;
    using DataTransferObjects;

    /// <summary>
    /// Custom Membership provider to override reset password functionality.
    /// </summary>
    public sealed class CustomMembershipProvider : SqlMembershipProvider
    {

        public override void Initialize(string name, NameValueCollection config)
        {

            string IsLockOutPolicyEnabled = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsLockOutPolicyEnabledKey);
            string maxInvalidPasswordAttempts = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FailedPasswordAttemptCountKey);
            string passwordAttemptWindow = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.PasswordAttemptWindowKey);


            bool result = true;
            bool.TryParse(IsLockOutPolicyEnabled, out result);

            if (!string.IsNullOrEmpty(IsLockOutPolicyEnabled) && !string.IsNullOrEmpty(maxInvalidPasswordAttempts) && !string.IsNullOrEmpty(passwordAttemptWindow))
            {
                if (result)
                {
                    config.Remove("maxInvalidPasswordAttempts");
                    config.Remove("passwordAttemptWindow");
                    config.Add("maxInvalidPasswordAttempts", maxInvalidPasswordAttempts);
                    config.Add("passwordAttemptWindow", passwordAttemptWindow);
                }

            }

            if (!result)
            {
                config.Remove("maxInvalidPasswordAttempts");
                config.Remove("passwordAttemptWindow");
                //Practically not possible to exceed maxInvalidPasswordAttempts more than Int32.MaxValue within 1 minute.
                config.Add("maxInvalidPasswordAttempts", "2147483647");
                config.Add("passwordAttemptWindow", "1");
            }

            base.Initialize(name, config);
        }

        public override string GeneratePassword()
        {
            string password = base.GeneratePassword();
            return PasswordGeneratorUtility.GeneratePassword(password);
        }

    }
}

