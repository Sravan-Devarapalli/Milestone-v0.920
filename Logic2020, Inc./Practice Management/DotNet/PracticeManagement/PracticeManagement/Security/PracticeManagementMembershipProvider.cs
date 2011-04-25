using System.ServiceModel;
using System.Web.Security;
using PraticeManagement.AuthService;
using PraticeManagement.MembershipService;

namespace PraticeManagement.Security
{
	/// <summary>
	/// Provides a Membership functionality working through the WCF Services.
	/// </summary>
	public class PracticeManagementMembershipProvider : MembershipProvider
	{
		#region MembershipProvider members

		/// <summary>
		/// Gets or sets a name of the current application.
		/// </summary>
		public override string ApplicationName
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetApplicationName();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
			set
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						serviceClient.SetApplicationName(value);
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Changes the password for the specified user.
		/// </summary>
		/// <param name="username">The name of the user to update the password for.</param>
		/// <param name="oldPassword">The current password for the specified user.</param>
		/// <param name="newPassword">The new password for the specified user.</param>
		/// <returns>true if the password was updated successfully; otherwise, false.</returns>
		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.ChangePassword(username, oldPassword, newPassword);
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					serviceClient.Abort();
					throw new MembershipPasswordException(ex.Detail.Message, ex);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Processes a request to update the password question and answer for a membership user.
		/// </summary>
		/// <param name="username">The user to change the password question and answer for.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <param name="newPasswordQuestion">The new password question for the specified user.</param>
		/// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
		/// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return
						serviceClient.ChangePasswordQuestionAndAnswer(
						username,
						password,
						newPasswordQuestion,
						newPasswordAnswer);
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					serviceClient.Abort();
					throw new MembershipPasswordException(ex.Detail.Message, ex);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
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
		public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return
						serviceClient.CreateUser(
						out status,
						username,
						password,
						email,
						passwordQuestion,
						passwordAnswer,
						isApproved,
						providerUserKey);
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					serviceClient.Abort();
					throw new MembershipCreateUserException(ex.Detail.Message, ex);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Removes a user from the membership data source.
		/// </summary>
		/// <param name="username">The name of the user to delete.</param>
		/// <param name="deleteAllRelatedData">
		/// true to delete data related to the user from the database; false to leave data related to the user in the database.
		/// </param>
		/// <returns>true if the user was successfully deleted; otherwise, false.</returns>
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.DeleteUser(username, deleteAllRelatedData);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the MembershipProvider instance is configured to allow users to reset
		/// their passwords.
		/// </summary>
		public override bool EnablePasswordReset
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetEnablePasswordReset();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user's password can be retrieved from the database.
		/// </summary>
		public override bool EnablePasswordRetrieval
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetEnablePasswordRetrieval();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
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
		public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					System.Web.Security.MembershipUserCollection result =
						serviceClient.FindUsersByEmail(out totalRecords, emailToMatch, pageIndex, pageSize);
					return UpdateMembershipProvider(result);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
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
		public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					System.Web.Security.MembershipUserCollection result =
						serviceClient.FindUsersByName(out totalRecords, usernameToMatch, pageIndex, pageSize);
					return UpdateMembershipProvider(result);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
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
		public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					System.Web.Security.MembershipUserCollection result =
						serviceClient.GetAllUsers(out totalRecords, pageIndex, pageSize);
					return UpdateMembershipProvider(result);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Throws a NotSupportedException exception in all cases.
		/// </summary>
		/// <returns>A NotSupportedException in all cases.</returns>
		/// <remarks>May be implemented in future.</remarks>
		public override int GetNumberOfUsersOnline()
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.GetNumberOfUsersOnline();
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Returns the password of the specified user from the database.
		/// </summary>
		/// <param name="username">The user to retrieve the password for.</param>
		/// <param name="passwordAnswer">The password answer for the user.</param>
		/// <returns></returns>
		public override string GetPassword(string username, string answer)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.GetPassword(username, answer);
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					serviceClient.Abort();
					throw new MembershipPasswordException(ex.Detail.Message, ex);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Gets the membership user information associated with the specified user name.
		/// </summary>
		/// <param name="username">The name of the user to get information for.</param>
		/// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
		/// <returns>Gets the membership user information associated with the specified user name.</returns>
        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					System.Web.Security.MembershipUser result = serviceClient.GetUser(username, userIsOnline);
					return UpdateMembershipProvider(result);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Changes the name of the Membership provider.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private System.Web.Security.MembershipUser UpdateMembershipProvider(System.Web.Security.MembershipUser user)
		{
			System.Web.Security.MembershipUser result =
				user != null ?
					new System.Web.Security.MembershipUser(Membership.Provider.Name,
						user.UserName,
						user.ProviderUserKey,
						user.Email,
						user.PasswordQuestion,
						user.Comment,
						user.IsApproved,
						user.IsLockedOut,
						user.CreationDate,
						user.LastLoginDate,
						user.LastActivityDate,
						user.LastPasswordChangedDate,
						user.LastLockoutDate) : null;

			return result;
		}

		/// <summary>
		/// Changes the name of the Membership provider.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private System.Web.Security.MembershipUserCollection UpdateMembershipProvider(System.Web.Security.MembershipUserCollection users)
		{
			System.Web.Security.MembershipUserCollection result = new System.Web.Security.MembershipUserCollection();
			foreach (System.Web.Security.MembershipUser user in users)
			{
				result.Add(UpdateMembershipProvider(user));
			}

			return result;
		}

		/// <summary>
		/// Gets the membership user information associated with the specified user key.
		/// </summary>
		/// <param name="providerUserKey">The unique identifier for the user.</param>
		/// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
		/// <returns>Gets the membership user information associated with the specified user key.</returns>
		public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					System.Web.Security.MembershipUser result = serviceClient.GetUserByProviderUserKey(providerUserKey, userIsOnline);
					return UpdateMembershipProvider(result);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Gets the user name associated with the specified e-mail address.
		/// </summary>
		/// <param name="email">The e-mail address to search for.</param>
		/// <returns>The user name associated with the specified e-mail address.</returns>
		public override string GetUserNameByEmail(string email)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.GetUserNameByEmail(email);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Gets the number of failed answer attempts a user is allowed for the password-reset question.
		/// </summary>
		public override int MaxInvalidPasswordAttempts
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetMaxInvalidPasswordAttempts();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets the minimum number of special characters that must be present in a valid password.
		/// </summary>
		public override int MinRequiredNonAlphanumericCharacters
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetMinRequiredNonAlphanumericCharacters();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets the minimum length required for a password.
		/// </summary>
		public override int MinRequiredPasswordLength
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetMinRequiredPasswordLength();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets the time window during which consecutive failed attempts to provide a valid password
		/// or a valid password answer are tracked.
		/// </summary>
		public override int PasswordAttemptWindow
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetPasswordAttemptWindow();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the format of passwords in the Active Directory data store.
		/// </summary>
		public override System.Web.Security.MembershipPasswordFormat PasswordFormat
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetPasswordFormat();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets the regular expression used to evaluate a password.
		/// </summary>
		public override string PasswordStrengthRegularExpression
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetPasswordStrengthRegularExpression();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the membership provider is configured to require a password question
		/// and answer when creating a user.
		/// </summary>
		public override bool RequiresQuestionAndAnswer
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetRequiresQuestionAndAnswer();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether an e-mail address stored on the database server must be unique.
		/// </summary>
		public override bool RequiresUniqueEmail
		{
			get
			{
				using (MembershipServiceClient serviceClient = new MembershipServiceClient())
				{
					try
					{
						return serviceClient.GetRequiresUniqueEmail();
					}
					catch (CommunicationException)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Resets a user's password to a new, automatically generated password.
		/// </summary>
		/// <param name="username">The user to reset the password for.</param>
		/// <param name="answer">The password answer for the specified user.</param>
		/// <returns>The new password for the specified user.</returns>
		public override string ResetPassword(string username, string answer)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.ResetPassword(username, answer);
				}
				catch (FaultException<ExceptionDetail> ex)
				{
					serviceClient.Abort();
					throw new MembershipPasswordException(ex.Detail.Message, ex);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Clears a lock so that a membership user can be validated.
		/// </summary>
		/// <param name="userName">The name of the membership user to clear the lock status for.</param>
		/// <returns>
		/// true if the membership user was successfully unlocked; otherwise, false.
		/// The UnlockUser method also returns false when the membership user is not found in the data store.
		/// </returns>
		public override bool UnlockUser(string userName)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					return serviceClient.UnlockUser(userName);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Updates information about a user in the Active Directory data store.
		/// </summary>
		/// <param name="user">
		/// A MembershipUser instance representing the user to update and the updated information for the user.
		/// </param>
		public override void UpdateUser(System.Web.Security.MembershipUser user)
		{
			using (MembershipServiceClient serviceClient = new MembershipServiceClient())
			{
				try
				{
					serviceClient.UpdateUser(user);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		/// <summary>
		/// Verifies that the specified user name and password exist in the database.
		/// </summary>
		/// <param name="username">The name of the user to validate.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <returns>
		/// true if the specified username and password are valid; otherwise, false.
		/// If the user specified does not exist in the database, the ValidateUser method returns false.
		/// </returns>
		public override bool ValidateUser(string username, string password)
		{
			using (AuthServiceClient serviceClient = new AuthServiceClient())
			{
				try
				{
					return serviceClient.ValidateUser(username, password);
				}
				catch (CommunicationException)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}
		
		#endregion
	}
}

