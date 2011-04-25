using System;
using System.Configuration;
using System.Net.Mail;
using System.ServiceModel.Activation;
using System.Web.Security;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    // NOTE: If you change the class name "MembershipService" here, you must also update the reference to "MembershipService" in Web.config.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MembershipService : IMembershipService
    {
        /// <summary>
        /// Gets the name of the application using the custom membership provider.
        /// </summary>
        /// <returns></returns>
        public string GetApplicationName()
        {
            return Membership.ApplicationName;
        }

        /// <summary>
        /// Sets the name of the application using the custom membership provider.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        public void SetApplicationName(string applicationName)
        {
            Membership.ApplicationName = applicationName;
        }

        /// <summary>
        /// Changes the password for the specified user.
        /// </summary>
        /// <param name="username">The name of the user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>true if the password was updated successfully; otherwise, false.</returns>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            MembershipUser user = Membership.GetUser(username);
            bool result = user != null ? user.ChangePassword(oldPassword, newPassword) : false;

            return result;
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            MembershipUser user = Membership.GetUser(username);
            return
                user != null ?
                user.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer) : false;
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user.</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">
        /// The unique identifier from the membership data source for the user.
        /// </param>
        /// <param name="status">
        /// A MembershipCreateStatus enumeration value indicating whether the user was created successfully.
        /// </param>
        /// <returns>A MembershipUser object populated with the information for the newly created user.</returns>
        public MembershipUser CreateUser(string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            Object providerUserKey,
            out MembershipCreateStatus status)
        {
            return Membership.CreateUser(username,
                password,
                email,
                passwordQuestion,
                passwordAnswer,
                isApproved,
                providerUserKey,
                out status);
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">
        /// true to delete data related to the user from the database; false to leave data related to the user in the database.
        /// </param>
        /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return Membership.DeleteUser(username, deleteAllRelatedData);
        }

        /// <summary>
        /// Gets a value indicating whether the MembershipProvider instance is configured to allow users to reset
        /// their passwords.
        /// </summary>
        /// <returns></returns>
        public bool GetEnablePasswordReset()
        {
            return Membership.EnablePasswordReset;
        }

        /// <summary>
        /// Gets a value indicating whether the user's password can be retrieved from the database.
        /// </summary>
        /// <returns></returns>
        public bool GetEnablePasswordRetrieval()
        {
            return Membership.EnablePasswordRetrieval;
        }

        /// <summary>
        /// Returns a collection of membership users from the database based on the user's e-mail address.
        /// </summary>
        /// <param name="emailToMatch">E-mail address or portion of e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">
        /// When this method returns, contains the total number of users returned in the collection.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// A MembershipUserCollection containing pageSizeMembershipUser instances beginning at the page specified by pageIndex.
        /// </returns>
        public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return Membership.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Returns a collection of users from the Active Directory data store based on the user name.
        /// </summary>
        /// <param name="usernameToMatch">The user name or portion of a user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">
        /// When this method returns, contains the total number of users returned in the collection.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// When this method returns, contains the total number of records returned in the collection.
        /// This parameter is passed uninitialized.
        /// </returns>
        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return Membership.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of all the users stored in an database.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">
        /// When this method returns, contains the total number of users returned in the collection.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// A MembershipUserCollection containing pageSizeMembershipUser instances beginning at the page specified by pageIndex.
        /// </returns>
        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return Membership.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Throws a NotSupportedException exception in all cases.
        /// </summary>
        /// <returns>A NotSupportedException in all cases.</returns>
        /// <remarks>May be implemented in future.</remarks>
        public int GetNumberOfUsersOnline()
        {
            return Membership.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// Returns the password of the specified user from the database.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="passwordAnswer">The password answer for the user.</param>
        /// <returns></returns>
        public string GetPassword(string username, string passwordAnswer)
        {
            MembershipUser user = Membership.GetUser(username);
            if (user != null)
            {
                return user.GetPassword(passwordAnswer);
            }
            else
            {
                throw new MembershipPasswordException();
            }
        }

        /// <summary>
        /// Gets the membership user information associated with the specified user name.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
        /// <returns>Gets the membership user information associated with the specified user name.</returns>
        public MembershipUser GetUser(string username, bool userIsOnline)
        {
            return Membership.GetUser(username, userIsOnline);
        }

        /// <summary>
        /// Gets the membership user information associated with the specified user key.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the user.</param>
        /// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
        /// <returns>Gets the membership user information associated with the specified user key.</returns>
        public MembershipUser GetUserByProviderUserKey(object providerUserKey, bool userIsOnline)
        {
            return Membership.GetUser(providerUserKey, userIsOnline);
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>The user name associated with the specified e-mail address.</returns>
        public string GetUserNameByEmail(string email)
        {
            return Membership.GetUserNameByEmail(email);
        }

        /// <summary>
        /// Gets the number of failed answer attempts a user is allowed for the password-reset question.
        /// </summary>
        /// <returns></returns>
        public int GetMaxInvalidPasswordAttempts()
        {
            return Membership.MaxInvalidPasswordAttempts;
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns></returns>
        public int GetMinRequiredNonAlphanumericCharacters()
        {
            return Membership.MinRequiredNonAlphanumericCharacters;
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns></returns>
        public int GetMinRequiredPasswordLength()
        {
            return Membership.MinRequiredPasswordLength;
        }

        /// <summary>
        /// Gets the time window during which consecutive failed attempts to provide a valid password
        /// or a valid password answer are tracked.
        /// </summary>
        /// <returns></returns>
        public int GetPasswordAttemptWindow()
        {
            return Membership.PasswordAttemptWindow;
        }

        /// <summary>
        /// Gets a value indicating the format of passwords in the Active Directory data store.
        /// </summary>
        /// <returns></returns>
        public MembershipPasswordFormat GetPasswordFormat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns></returns>
        public string GetPasswordStrengthRegularExpression()
        {
            return Membership.PasswordStrengthRegularExpression;
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a password question
        /// and answer when creating a user.
        /// </summary>
        /// <returns></returns>
        public bool GetRequiresQuestionAndAnswer()
        {
            return Membership.RequiresQuestionAndAnswer;
        }

        /// <summary>
        /// Gets a value indicating whether an e-mail address stored on the database server must be unique.
        /// </summary>
        /// <returns></returns>
        public bool GetRequiresUniqueEmail()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public string ResetPassword(string username, string answer)
        {
            MembershipUser user = Membership.GetUser(username);
            string result;
            if (user != null)
            {
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                result = user.ResetPassword(answer);

            }
            else
            {
                throw new MembershipPasswordException();
            }

            Person person = PersonDAL.PersonGetByAlias(username);
            if (person != null)
            {
                try
                {
                    MailUtil.SendNotification(
                        person,
                        result,
                        ConfigurationManager.AppSettings[MailUtil.ResetPasswordEmailTemplateKey]);
                }
                catch (SmtpException)
                {
                    // We ignore this exception here according to the task #808
                }
            }

            return result;
        }

        /// <summary>
        /// Clears a lock so that a membership user can be validated.
        /// </summary>
        /// <param name="userName">The name of the membership user to clear the lock status for.</param>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// The UnlockUser method also returns false when the membership user is not found in the data store.
        /// </returns>
        public bool UnlockUser(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            return user != null ? user.UnlockUser() : false;
        }

        /// <summary>
        /// Updates information about a user in the Active Directory data store.
        /// </summary>
        /// <param name="user">
        /// A MembershipUser instance representing the user to update and the updated information for the user.
        /// </param>
        public void UpdateUser(MembershipUser user)
        {
            Membership.UpdateUser(user);
        }
    }
}

