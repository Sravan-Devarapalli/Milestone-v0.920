using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
    [ServiceKnownType(typeof(Timescale))]
    [ServiceContract]
    public interface IPersonService
    {
        /// <summary>
        /// Person Milestone With Financials
        /// </summary>
        [OperationContract]
        DataSet GetPersonMilestoneWithFinancials(int personId);

        /// <summary>
        /// Set the person as default manager
        /// </summary>
        [OperationContract]
        void SetAsDefaultManager(Person person);

       /// <summary>
        /// Checks if the person is a manager to somebody
        /// </summary>
        [OperationContract]
        bool IsSomeonesManager(Person person);

        /// <summary>
        /// Set new manager
        /// </summary>
        [OperationContract]
        void SetNewManager(Person oldManager, Person newManager);

        /// <summary>
        /// Retrieves consultants report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        [OperationContract]
        DataSet GetConsultantUtilizationReport(ConsultantTableReportContext context);

        /// <summary>
        /// Retrieves consultants report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        [OperationContract]
        List<Quadruple<Person, int[], int, int>> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context);

        /// <summary>
        /// Retrieves a consultant's  daily report whose Oersin Id is given by personId.
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        [OperationContract]
        List<Triple<Person, int[], int>> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context);

        /// <summary>
        /// Check's if there's compensation record covering milestone/
        /// See #886 for details.
        /// </summary>
        /// <param name="person">Person to check against</param>
        /// <returns>True if there's such record, false otherwise</returns>
        [OperationContract]
        bool IsCompensationCoversMilestone(Person person, DateTime? start, DateTime? end);

        /// <summary>
        /// Verifies whether a user has compensation at this moment
        /// </summary>
        /// <param name="personId">Id of the person</param>
        /// <returns>True if a person has active compensation, false otherwise</returns>
        [OperationContract]
        bool CurrentPayExists(int personId);

        /// <summary>
        /// Retrieves a list of the persons for excel export.
        /// </summary>
        /// <returns>A list of the <see cref="Opportunity"/> objects.</returns>
        [OperationContract]
        System.Data.DataSet PersonGetExcelSet();
		/// <summary>
        /// <summary>
        /// Gets all permissions for the given person
        /// </summary>
        /// <param name="person">Person to get permissions for</param>
        /// <returns>Object with the list of permissions</returns>
        [OperationContract]
        PersonPermission GetPermissions(Person person);

        /// <summary>
		/// Get all persons
		/// </summary>
		/// <param name="practice">identifies a practice to limit results</param>
		/// <param name="active"><value>true</value> limits to active persons only</param>
		/// <param name="looked">A text to be looked for.</param>
		/// <param name="pageNo">Determines a page index to be retrieved.</param>
		/// <param name="pageSize">Determines a page size to be retrieved.</param>
		/// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
		/// <param name="userName">A current user.</param>
		/// <returns>A list of <see cref="Person"/>s in the system matching filters</returns>
		[OperationContract]
		List<Person> GetPersonList(
			int? practice,
			bool active,
			int pageSize,
			int pageNo,
			string looked,
			int? recruiterId,
			string userName);

        /// <summary>
        /// This method is used by only in PersonList.aspx, Set_user.aspx and DataHelper.cs. This will return 
        /// all person list along with his currentPay details.
        /// </summary>
        /// <param name="practice">identifies a practice to limit results</param>
        /// <param name="active"><value>true</value> limits to active persons only</param>
        /// <param name="looked">A text to be looked for.</param>
        /// <param name="pageNo">Determines a page index to be retrieved.</param>
        /// <param name="pageSize">Determines a page size to be retrieved.</param>
        /// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
        /// <param name="userName">A current user.</param>
        /// <param name="sortBy">The sort column by which the result set should be sorted.</param>
        /// <returns></returns>
        [OperationContract]
        List<Person> GetPersonListWithCurrentPay(
            int? practice,
            bool active,
            int pageSize,
            int pageNo,
            string looked,
            int? recruiterId,
            string userName,
            string sortBy);

		/// <summary>
		/// Get all persons able to work on Milestone
		/// </summary>
		/// <param name="practice">identifies a practice to limit results</param>
		/// <param name="active"><value>true</value> limits to active persons only</param>        
		/// <param name="pageNo">Determines a page index to be retrieved.</param>
		/// <param name="pageSize">Determines a page size to be retrieved.</param>
		/// <param name="startDate">Determines a start date when persons in the list must are available.</param>
		/// <param name="endDate">Determines an end date when persons in the list must are available.</param>
		/// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
		/// <param name="userName">A current user.</param>
		/// <returns>A list of <see cref="Person"/>s in the system matching filters</returns>
		[OperationContract]
        List<Person> GetPersonListActiveDate(
			int? practice,
			bool active,
			int pageSize,
			int pageNo,
			string looked,
			DateTime startDate,
			DateTime endDate,
			int? recruiterId,
			string userName);

        /// <summary>
        /// Retrieves a short info on persons.
        /// </summary>
        /// <param name="practice">Practice filter, null meaning all practices</param>
        /// <param name="statusId">Person status id</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        [OperationContract]
        List<Person> PersonListAllShort(int? practice, int? statusId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves a short info on persons.
        /// </summary>
        /// <param name="seniorityId">Person seniority id</param>
        /// <param name="statusId">Person status id</param>
        /// <returns></returns>
        [OperationContract]
        List<Person> PersonsGetBySeniorityAndStatus(int seniorityId, int? statusId);

        /// <summary>
        ///  Retrieves a short info on persons.
        /// </summary>
        /// <param name="statusId">Person status id</param>
        /// <param name="roleName">Person role</param>
        /// <returns>A list of the <see cref="Person"/> objects</returns>
        [OperationContract]
        List<Person> PersonListShortByRoleAndStatus(int? statusId, string roleName);
		/// <summary>
		/// Retrieves a short info on persons who are not in the Administration practice.
		/// </summary>
		/// <param name="milestonePersonId">An ID of existing milestone-person association or null.</param>
		/// <param name="startDate">Determines a start date when persons in the list must are available.</param>
		/// <param name="endDate">Determines an end date when persons in the list must are available.</param>
		/// <returns>A list of the <see cref="Person"/> objects.</returns>
		[OperationContract]
		List<Person> PersonListAllForMilestone(int? milestonePersonId, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Calculates a number of <see cref="Person"/>s match with the specified conditions.
		/// </summary>
		/// <param name="practice">The <see cref="Person"/>'s default practice.</param>
		/// <param name="showAll">List all <see cref="Person"/>s if true and the only active otherwise.</param>
		/// <param name="looked">List all <see cref="Person"/>s by search string that matches for first name or last name  .</param>
		/// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
		/// <param name="userName">A current user.</param>
		/// <returns>The number of the persons those match with the specified conditions.</returns>
		[OperationContract]
		int GetPersonCount(int? practice, bool showAll, string looked, int? recruiterId, string userName);

        /// <summary>
        /// Calculates a number of <see cref="Person"/>s working days in period.
        /// </summary>
        /// <param name="personId">Id of the person to get</param>
        /// <param name="startDate">mileStone start date </param>
        /// <param name="endDate">mileStone end date</param>
        /// <returns>The number of the persons working days in period.</returns>
        [OperationContract]
        int GetPersonWorkDaysNumber(int personId, DateTime startDate, DateTime endDate);        

		/// <summary>
		/// Lists all active persons who receive some recruiter commissions.
		/// </summary>
		/// <returns>The list of <see cref="Person"/> objects.</returns>
		[OperationContract]
		List<Person> GetRecruiterList(int? personId, DateTime? hireDate);

		/// <summary>
		/// List the persons who recieve the sales commissions
		/// </summary>
		/// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
		/// <returns>The list of the <see cref="Person"/> objects.</returns>
		[OperationContract]
		List<Person> GetSalespersonList(bool includeInactive);

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="person">Person to restrict permissions to</param>
        /// <param name="inactives">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        [OperationContract]
        List<Person> PersonListSalesperson(Person person, bool inactives);

		/// <summary>
		/// List the persons who recieve the Practice Management commissions
		/// </summary>
		/// <param name="projectId">An ID of the project to the Practice Manager be selected for.</param>
		/// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
		/// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
		/// <returns>
		/// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
		/// </returns>
		[OperationContract]
		List<Person> GetPracticeManagerList(int? projectId, DateTime? endDate, bool includeInactive);

        /// <summary>
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person"></param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        [OperationContract]
        List<Person> PersonListProjectOwner(DateTime? endDate, bool includeInactive, Person person);

        /// <summary>
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="projectId">An ID of the project to the Practice Manager be selected for.</param>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person">Person to apply restrictions to</param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        [OperationContract]
        List<Person> PersonListPracticeManager(Person person, int? projectId, DateTime? endDate, bool includeInactive);

		/// <summary>
		/// Retrieves all subordinated persons for a specified practice manager.
		/// </summary>
		/// <param name="practiceManagerId">An ID of the parctice manager to teh data be retrieved for.</param>
		/// <returns>The list of the <see cref="Person"/> objects.</returns>
		[OperationContract]
		List<Person> GetSubordinates(int practiceManagerId);

        /// <summary>
        /// Retrieves Person One-off List.
        /// </summary>
        /// <param name="today">An ID of the parctice manager to teh data be retrieved for.</param>
        /// <param name="userName">Current user alias</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        [OperationContract]
        List<Person> GetOneOffList(DateTime today, string userName);

		/// <summary>
        /// Get a person
        /// </summary>
        /// <param name="personId">Id of the person to get</param>
        /// <returns>
		/// Person matching <paramref name="personId"/>, or <value>null</value> if the person is not in the system
		/// </returns>
        /// <remarks>
        /// Presumably the id is obtained form a previous call to GetPersonList but
        /// there is no system restriction on the value for the identifier in this call.
        /// </remarks>
        [OperationContract]
        Person GetPersonDetail(int personId);

		/// <summary>
		/// Retrieves a <see cref="Person"/> by the Alias (email).
		/// </summary>
		/// <param name="alias">The EMail to search for.</param>
		/// <returns>The <see cref="Person"/> object if found and null otherwise.</returns>
		[OperationContract]
		Person GetPersonByAlias(string alias);

        // TODO: better define "if the person exists"  The id is new or zero, maybe?
        /// <summary>
        /// Commit data about a <see cref="Person"/> to the system store
        /// </summary>
        /// <param name="person">Person information to store</param>
        /// <remarks>
        /// If the person exists in the system then this information overwirtes information already in
        /// the store, otherwise a new person is created, and an identifier is placed
        /// in the <paramref name="person"/>
        /// </remarks>
        /// <param name="currentUser">current logged in user name</param>       
        [OperationContract]
		[FaultContract(typeof(DataAccessFault))]
        int SavePersonDetail(Person person, string currentUser);

		/// <summary>
		/// Deactivates the specified person.
		/// </summary>
		/// <param name="person">The data for the person to be deactivated.</param>
		[OperationContract]
		void PersonInactivate(Person person);

		/// <summary>
		/// Activates the specified person.
		/// </summary>
		/// <param name="person">The data for the person to be activated.</param>
		[OperationContract]
		void PersonReactivate(Person person);

		/// <summary>
		/// Retrieves the list of the overheads for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrive the data for.</param>
		/// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
		[OperationContract]
		List<PersonOverhead> GetPersonOverheadByPerson(int personId);

		/// <summary>
		/// Retrieves a list of overheads for the specified <see cref="Timescale"/>.
		/// </summary>
		/// <param name="timescale">The <see cref="Timescale"/> to retrive the data for.</param>
		/// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
		[OperationContract]
		List<PersonOverhead> GetPersonOverheadByTimescale(TimescaleType timescale);

		/// <summary>
		/// Retrieves the person's rate.
		/// </summary>
		/// <param name="milestonePerson">The milestone-person association to the rate be calculated for.</param>
		/// <returns>The <see cref="MilestonePerson"/> object with calculated data.</returns>
		[OperationContract]
		MilestonePerson GetPersonRate(MilestonePerson milestonePerson);

		/// <summary>
		/// Calculates the person's financials.
		/// </summary>
		/// <param name="personId">An ID of the <see cref="Person"/> to colculate the data for.</param>
		/// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
		/// <param name="proposedRate">A proposed person's hourly rate.</param>
		/// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
		[OperationContract]
        ComputedFinancialsEx CalculateProposedFinancials(int personId, decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount);

		/// <summary>
		/// Calculates the person's rate.
		/// </summary>
		/// <param name="person">A <see cref="Person"/> object to calculate the data for.</param>
		/// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
		/// <param name="proposedRate">A proposed person's hourly rate.</param>
		/// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
		[OperationContract]
        ComputedFinancialsEx CalculateProposedFinancialsPerson(Person person, decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount, bool isMarginTestPage);

		/// <summary>
		/// Calculates the person's rate.
		/// </summary>
		/// <param name="person">A <see cref="Person"/> object to calculate the data for.</param>
		/// <param name="targetMargin">A Target Magin.</param>
		/// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
		/// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
		[OperationContract]
        ComputedFinancialsEx CalculateProposedFinancialsPersonTargetMargin(Person person, decimal targetMargin, decimal proposedHoursPerWeek, decimal clientDiscount, bool isMarginTestPage);

		/// <summary>
		/// Pertieves a payment for the specified person.
		/// </summary>
		/// <param name="personId">The <see cref="Person"/> to the data be retrieved for.</param>
		/// <param name="startDate">The StartDate since the payment is active.</param>
		/// <returns>The <see cref="Pay"/> object when found and null otherwise.</returns>
		[OperationContract]
		Pay GetPayment(int personId, DateTime startDate);

		/// <summary>
		/// Saves a payment data.
		/// </summary>
		/// <param name="pay">The <see cref="Pay"/> object to be saved.</param>
		[OperationContract]
		void SavePay(Pay pay);

		/// <summary>
		/// Selects a list of the seniorities.
		/// </summary>
		/// <returns>A list of the <see cref="Seniority"/> objects.</returns>
		[OperationContract]
		List<Seniority> ListSeniorities();

        /// <summary>
        /// Sets permissions for user
        /// </summary>
        /// <param name="person">Person to set permissions for</param>
        /// <param name="permissions">Permissions to set for the user</param>
        [OperationContract]
        void SetPermissionsForPerson(Person person, PersonPermission permissions);

        /// <summary>
        /// Get person by it's ID
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>Person</returns>
        [OperationContract]
        Person GetPersonById(int personId);

        /// <summary>
        /// Lists managers subordinates
        /// </summary>
        /// <param name="person">Manager</param>
        /// <returns>List of subordinates</returns>
        [OperationContract]
        Person[] ListManagersSubordinates(Person person);
        
        /// <summary>
        /// Retrives a short info on persons.
        /// </summary>
        /// <param name="statusList">Comma seperated statusIds</param>
        /// <returns></returns>
        [OperationContract]
        List<Person> GetPersonListByStatusList(string statusList, int? personId);

        /// <summary>
        /// Retrives a short info of persons specified by personIds.
        /// </summary>
        /// <param name="statusList">Comma seperated PersonIds</param>
        /// <returns></returns>
        [OperationContract]
        List<Person> GetPersonListByPersonIdList(string PersonIds);

        [OperationContract]
        bool SaveUserTemporaryCredentials(string userName, string PMLoginPageUrl, string PMChangePasswordPageUrl);

        [OperationContract]
        bool CheckIfTemporaryCredentialsValid(string userName, string password);

        [OperationContract]
        void SetNewPasswordForUser(string userName, string newPassword);

        [OperationContract]
        List<Person> PersonListByCategoryTypeAndPeriod(BudgetCategoryType categoryType, DateTime startDate, DateTime endDate);
    }
}

