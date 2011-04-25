using System;
using System.ServiceModel;
using System.Web.Security;

namespace PracticeManagementService
{
	// NOTE: If you change the interface name "IMembershipService" here, you must also update the reference to "IMembershipService" in Web.config.
	[ServiceContract]
	public interface IMembershipService
	{
		/// <summary>
		/// Gets the name of the application using the custom membership provider.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		string GetApplicationName();

		/// <summary>
		/// Sets the name of the application using the custom membership provider.
		/// </summary>
		/// <param name="applicationName">The name of the application.</param>
		[OperationContract]
		void SetApplicationName(string applicationName);

		/// <summary>
		/// Changes the password for the specified user.
		/// </summary>
		/// <param name="username">The name of the user to update the password for.</param>
		/// <param name="oldPassword">The current password for the specified user.</param>
		/// <param name="newPassword">The new password for the specified user.</param>
		/// <returns>true if the password was updated successfully; otherwise, false.</returns>
		[OperationContract]
		bool ChangePassword(string username, string oldPassword, string newPassword);

		/// <summary>
		/// Processes a request to update the password question and answer for a membership user.
		/// </summary>
		/// <param name="username">The user to change the password question and answer for.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <param name="newPasswordQuestion">The new password question for the specified user.</param>
		/// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
		/// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
		[OperationContract]
		bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer);

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
		[OperationContract]
		MembershipUser CreateUser(string username,
			string password,
			string email,
			string passwordQuestion,
			string passwordAnswer,
			bool isApproved,
			Object providerUserKey,
			out MembershipCreateStatus status);

		/// <summary>
		/// Removes a user from the membership data source.
		/// </summary>
		/// <param name="username">The name of the user to delete.</param>
		/// <param name="deleteAllRelatedData">
		/// true to delete data related to the user from the database; false to leave data related to the user in the database.
		/// </param>
		/// <returns>true if the user was successfully deleted; otherwise, false.</returns>
		[OperationContract]
		bool DeleteUser(string username, bool deleteAllRelatedData);

		/// <summary>
		/// Gets a value indicating whether the MembershipProvider instance is configured to allow users to reset
		/// their passwords.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		bool GetEnablePasswordReset();

		/// <summary>
		/// Gets a value indicating whether the user's password can be retrieved from the database.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		bool GetEnablePasswordRetrieval();

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
		[OperationContract]
		MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);

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
		[OperationContract]
		MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

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
		[OperationContract]
		MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords);

		/// <summary>
		/// Throws a NotSupportedException exception in all cases.
		/// </summary>
		/// <returns>A NotSupportedException in all cases.</returns>
		/// <remarks>May be implemented in future.</remarks>
		[OperationContract]
		int GetNumberOfUsersOnline();

		/// <summary>
		/// Returns the password of the specified user from the database.
		/// </summary>
		/// <param name="username">The user to retrieve the password for.</param>
		/// <param name="passwordAnswer">The password answer for the user.</param>
		/// <returns></returns>
		[OperationContract]
		string GetPassword(string username, string passwordAnswer);

		/// <summary>
		/// Gets the membership user information associated with the specified user name.
		/// </summary>
		/// <param name="username">The name of the user to get information for.</param>
		/// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
		/// <returns>Gets the membership user information associated with the specified user name.</returns>
		[OperationContract]
		MembershipUser GetUser(string username, bool userIsOnline);

		/// <summary>
		/// Gets the membership user information associated with the specified user key.
		/// </summary>
		/// <param name="providerUserKey">The unique identifier for the user.</param>
		/// <param name="userIsOnline">The GetUser method ignores this parameter.</param>
		/// <returns>Gets the membership user information associated with the specified user key.</returns>
		[OperationContract]
		MembershipUser GetUserByProviderUserKey(object providerUserKey, bool userIsOnline);

		/// <summary>
		/// Gets the user name associated with the specified e-mail address.
		/// </summary>
		/// <param name="email">The e-mail address to search for.</param>
		/// <returns>The user name associated with the specified e-mail address.</returns>
		[OperationContract]
		string GetUserNameByEmail(string email);

		/// <summary>
		/// Gets the number of failed answer attempts a user is allowed for the password-reset question.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		int GetMaxInvalidPasswordAttempts();

		/// <summary>
		/// Gets the minimum number of special characters that must be present in a valid password.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		int GetMinRequiredNonAlphanumericCharacters();

		/// <summary>
		/// Gets the minimum length required for a password.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		int GetMinRequiredPasswordLength();

		/// <summary>
		/// Gets the time window during which consecutive failed attempts to provide a valid password
		/// or a valid password answer are tracked.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		int GetPasswordAttemptWindow();

		/// <summary>
		/// Gets a value indicating the format of passwords in the Active Directory data store.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		MembershipPasswordFormat GetPasswordFormat();

		/// <summary>
		/// Gets the regular expression used to evaluate a password.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		string GetPasswordStrengthRegularExpression();

		/// <summary>
		/// Gets a value indicating whether the membership provider is configured to require a password question
		/// and answer when creating a user.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		bool GetRequiresQuestionAndAnswer();

		/// <summary>
		/// Gets a value indicating whether an e-mail address stored on the database server must be unique.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		bool GetRequiresUniqueEmail();

		/// <summary>
		/// Resets a user's password to a new, automatically generated password.
		/// </summary>
		/// <param name="username">The user to reset the password for.</param>
		/// <param name="answer">The password answer for the specified user.</param>
		/// <returns>The new password for the specified user.</returns>
		[OperationContract]
		string ResetPassword(string username, string answer);

		/// <summary>
		/// Clears a lock so that a membership user can be validated.
		/// </summary>
		/// <param name="userName">The name of the membership user to clear the lock status for.</param>
		/// <returns>
		/// true if the membership user was successfully unlocked; otherwise, false.
		/// The UnlockUser method also returns false when the membership user is not found in the data store.
		/// </returns>
		[OperationContract]
		bool UnlockUser(string userName);

		/// <summary>
		/// Updates information about a user in the Active Directory data store.
		/// </summary>
		/// <param name="user">
		/// A MembershipUser instance representing the user to update and the updated information for the user.
		/// </param>
		[OperationContract]
		void UpdateUser(MembershipUser user);
	}
}

