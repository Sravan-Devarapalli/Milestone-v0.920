using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Security;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.Reports;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Utils;

namespace PracticeManagementService
{
    /// <summary>
    /// Person information supplied
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PersonService : IPersonService
    {
        #region IPersonService Members

        public DataSet GetPersonMilestoneWithFinancials(int personId)
        {
            return PersonDAL.GetPersonMilestoneWithFinancials(personId);
        }

        /// <summary>
        /// Set the person as default manager
        /// </summary>
        public void SetAsDefaultManager(int personId)
        {
            PersonDAL.SetAsDefaultManager(personId);
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public List<ConsultantUtilizationPerson> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context)
        {
            return PersonDAL.GetConsultantUtilizationWeekly(context);
        }

        public List<ConsultantUtilizationPerson> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context)
        {
            return PersonDAL.ConsultantUtilizationDailyByPerson(personId, context);
        }

        /// <summary>
        /// Gets all permissions for the given person
        /// </summary>
        /// <param name="person">Person to get permissions for</param>
        /// <returns>Object with the list of permissions</returns>
        public PersonPermission GetPermissions(Person person)
        {
            PersonPermission permission = PersonDAL.GetPermissions(person);
            return permission;
        }

        public List<Person> GetPersonListWithCurrentPayByCommaSeparatedIdsList(
            string practiceIdsSelected,
            string divisionIdsSelected,
            bool active,
            int pageSize,
            int pageNo,
            string looked,
            string recruiterIdsSelected,
            string userName,
            string sortBy,
            string timeScaleIdsSelected,
            bool projected,
            bool terminated,
            bool terminatedPending,
            char? alphabet)
        {
            PersonRateCalculator.VerifyPrivileges(userName, ref recruiterIdsSelected);
            return
                PersonDAL.PersonListFilteredWithCurrentPayByCommaSeparatedIdsList(practiceIdsSelected, divisionIdsSelected, !active, pageSize, pageNo, looked, DateTime.MinValue, DateTime.MinValue, recruiterIdsSelected, null, sortBy, timeScaleIdsSelected, projected, terminated, terminatedPending, alphabet);
        }

        /// <summary>
        /// Retrives <see cref="Person"/> data to be exported to excel.
        /// </summary>
        /// <returns>An <see cref="Person"/> object if found and null otherwise.</returns>
        public System.Data.DataSet PersonGetExcelSet()
        {
            System.Data.DataSet result =
                PersonDAL.PersonGetExcelSet();

            return result;
        }

        public System.Data.DataSet PersonGetExcelSetWithFilters(
            string practiceIdsSelected,
           string divisionIdsSelected,
           string looked,
           string recruiterIdsSelected,
           string timeScaleIdsSelected,
           bool Active,
           bool projected,
           bool terminated,
           bool terminatedPending
            )
        {
            return PersonDAL.PersonGetExcelSetWithFilters(practiceIdsSelected, divisionIdsSelected, looked, recruiterIdsSelected, timeScaleIdsSelected, Active, projected, terminated, terminatedPending);
        }
        /// <summary>
        /// Retrives a short info on persons.
        /// </summary>
        /// <param name="practice">Practice filter, null meaning all practices</param>
        /// <param name="statusIds">Person status ids</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        public List<Person> PersonListAllShort(int? practice, string statusIds, DateTime startDate, DateTime endDate)
        {
            return PersonDAL
                .PersonListAllShort(practice, statusIds, startDate, endDate)
                .OrderBy(p => p.LastName + p.FirstName)
                .ToList();
        }

        public List<Person> OwnerListAllShort(string statusIds)
        {
            return PersonDAL.OwnerListAllShort(statusIds);
        }

        /// <summary>
        ///  Retrieves a short info on persons.
        /// </summary>
        /// <param name="statusId">Person status id</param>
        /// <param name="roleName">Person role</param>
        /// <returns>A list of the <see cref="Person"/> objects</returns>
        public List<Person> PersonListShortByRoleAndStatus(string statusIds, string roleName)
        {
            return PersonDAL
                .PersonListShortByRoleAndStatus(statusIds, roleName)
                .OrderBy(p => p.LastName + p.FirstName)
                .ToList();
        }

        public List<Person> PersonListShortByTitleAndStatus(string statusIds, string titleNames)
        {
            return PersonDAL
                .PersonListShortByTitleAndStatus(statusIds, titleNames)
                .OrderBy(p => p.LastName + p.FirstName)
                .ToList();
        }

        /// <summary>
        /// Retrives a short info on persons.
        /// </summary>
        /// <param name="statusList">Comma seperated statusIds</param>
        /// <returns></returns>
        public List<Person> GetPersonListByStatusList(string statusList, int? personId)
        {
            return PersonDAL.GetPersonListByStatusList(statusList, personId);
        }

        /// <summary>
        /// Retrives a short info of persons specified by personIds.
        /// </summary>
        /// <param name="personIds"></param>
        /// <returns></returns>
        public List<Person> GetPersonListByPersonIdList(string PersonIds)
        {
            return PersonDAL.GetPersonListByPersonIdList(PersonIds);
        }

        public List<Person> GetPersonListByPersonIdsAndPayTypeIds(string personIds, string paytypeIds, string practiceIds, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.GetPersonListByPersonIdsAndPayTypeIds(personIds, paytypeIds, practiceIds, startDate, endDate);
        }

        /// <summary>
        /// Retrives a short info on persons who are not in the Administration practice.
        /// </summary>
        /// <param name="milestonePersonId">An ID of existing milestone-person association or null.</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        public List<Person> PersonListAllForMilestone(int? milestonePersonId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.PersonListAllForMilestone(milestonePersonId, startDate, endDate);
        }

        /// <summary>
        /// Calculates a number of <see cref="Person"/>s match with the specified conditions.
        /// </summary>
        /// <param name="practiceIds">List of practice Ids</param>
        /// <param name="active">Is active or not</param>
        /// <param name="looked">search text</param>
        /// <param name="recruiterIds">List of recruiter Ids</param>
        /// <param name="userName">Logged in user</param>
        /// <param name="timeScaleIds">List of Time scale Ids</param>
        /// <param name="projected">Is Contingent or not</param>
        /// <param name="terminated">Is terminatied or not</param>
        /// <param name="terminatedPending">Is Termination Pending or not</param>
        /// <param name="alphabet">person starts with the letter</param>
        /// <returns></returns>
        public int GetPersonCountByCommaSeperatedIdsList(string practiceIds, string divisionIdsSelected, bool active, string looked, string recruiterIds, string userName, string timeScaleIds, bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            PersonRateCalculator.VerifyPrivileges(userName, ref recruiterIds);
            return PersonDAL.PersonGetCount(practiceIds, divisionIdsSelected, !active, looked, recruiterIds, timeScaleIds, projected, terminated, terminatedPending, alphabet);
        }

        /// <summary>
        /// Calculates a number of <see cref="Person"/>s working days in period.
        /// </summary>
        /// <param name="personId">Id of the person to get</param>
        /// <param name="startDate">mileStone start date </param>
        /// <param name="endDate">mileStone end date</param>
        /// <returns>The number of the persons working days in period.</returns>
        public PersonWorkingHoursDetailsWithinThePeriod GetPersonWorkingHoursDetailsWithinThePeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.GetPersonWorkingHoursDetailsWithinThePeriod(personId, startDate, endDate);
        }

        /// <summary>
        /// Lists all active persons who receive some recruiter commissions.
        /// </summary>
        /// <returns>The list of <see cref="Person"/> objects.</returns>
        public List<Person> GetRecruiterList()
        {
            return PersonDAL.PersonListRecruiter();
        }

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public List<Person> GetSalespersonList(bool includeInactive)
        {
            return PersonDAL.PersonListSalesperson(null, includeInactive);
        }

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="person">Person to restrict permissions to</param>
        /// <param name="inactives">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public List<Person> PersonListSalesperson(Person person, bool inactives)
        {
            return PersonDAL.PersonListSalesperson(person, inactives);
        }

        /// <summary>
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person"></param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        public List<Person> PersonListProjectOwner(bool includeInactive, Person person)
        {
            return PersonDAL.PersonListProjectOwner(includeInactive, person);
        }

        /// <summary>
        /// Read All persons firstname and last name  except having inactive status and must have compensation for today or in future.
        /// </summary>
        /// <param name="today"></param>
        /// <returns>List<Person/></returns>
        public List<Person> GetOneOffList(DateTime today)
        {
            return PersonDAL.PersonOneOffList(today);
        }

        /// <summary>
        /// Get person by it's ID
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>Person</returns>
        public Person GetPersonById(int personId)
        {
            return PersonDAL.GetById(personId);
        }

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
        public Person GetPersonDetail(int personId)
        {
            Person result = PersonDAL.GetById(personId);
            if (result != null)
            {
                result.PaymentHistory = PayDAL.GetHistoryByPerson(personId);

                result.RoleNames =
                    !string.IsNullOrEmpty(result.Alias) ? Roles.GetRolesForUser(result.Alias) : new string[0];

                MembershipUser user = Membership.GetUser(result.Alias);
                result.LockedOut = user != null && user.IsLockedOut;
            }

            return result;
        }

        /// <summary>
        /// To get the history of person Hires to the company.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public List<Employment> GetPersonEmploymentHistoryById(int personId)
        {
            return PersonDAL.GetPersonEmploymentHistoryById(personId);
        }

        /// <summary>
        /// Retrives a <see cref="Person"/> by the Alias (email).
        /// </summary>
        /// <param name="alias">The EMail to search for.</param>
        /// <returns>The <see cref="Person"/> object if found and null otherwise.</returns>
        public Person GetPersonByAlias(string alias)
        {
            return PersonDAL.PersonGetByAlias(alias);
        }

        /// <summary>
        /// Commit data about a <see cref="Person"/> to the system store
        /// </summary>
        /// <param name="person">Person information to store</param>
        /// <remarks>
        /// If the person exists in the system then this information overwirtes information already in
        /// the store.  If the <paramref name="person"/>.id is null a new person is created, and an identifier
        /// is placed in the <paramref name="person"/>
        /// </remarks>
        /// <returns>An ID of the saved record.</returns>
        public int SavePersonDetail(Person person, string currentUser, string loginPageUrl, bool saveCurrentPay, string userLogin)
        {
            Person oldPerson = person.Id.HasValue ? PersonDAL.GetById(person.Id.Value) : null;
            try
            {
                ProcessPersonData(person, currentUser, oldPerson, loginPageUrl, saveCurrentPay, userLogin);
                if (person.Id != null) return person.Id.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return -1;
        }

        public static void SendWelcomeEmail(Person person, string companyName, string loginPageUrl)
        {
            try
            {
                MembershipUser user = Membership.GetUser(person.Alias);
                if (user == null)
                {
                    throw new MembershipPasswordException("User Not Exists.");
                }
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                string password = user.ResetPassword();
                MailUtil.SendWelcomeEmail(person.FirstName, person.Alias, password, companyName, loginPageUrl);
                PersonDAL.UpdateIsWelcomeEmailSentForPerson(person.Id);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "SendWelcomeEmail", "ProjectService.svc", string.Empty,
                    System.Web.HttpUtility.HtmlEncode(e.Message), e.Source, e.InnerException == null ? string.Empty : System.Web.HttpUtility.HtmlEncode(e.InnerException.Message), e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public static string EncodePassword(string pass, string salt)
        {
            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            // MembershipPasswordFormat.Hashed
            HashAlgorithm s = HashAlgorithm.Create(Membership.HashAlgorithmType);
            if (s != null) bRet = s.ComputeHash(bAll);
            return bRet != null ? Convert.ToBase64String(bRet) : null;
        }

        /// <summary>
        /// generates salt to encode password.
        /// </summary>
        /// <returns></returns>
        public static string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// Sends mails after processing person data.
        /// </summary>
        /// <param name="oldPerson">Old person data.</param>
        /// <param name="person">Present person data.</param>
        /// <param name="isRehireDueToPay">Is person rehire due to compensation change of contract to employee.</param>
        /// <param name="loginPageUrl">Login page url of site.</param>
        public static void SendMailsAfterProcessPersonData(Person oldPerson, Person person, bool isRehireDueToPay, string loginPageUrl, string userLogin)
        {
            int personStatusId = person.Status.Id;
            bool isPersonActive = (personStatusId == (int)PersonStatusType.Active || personStatusId == (int)PersonStatusType.TerminationPending);

            if (oldPerson != null)//updating person data.
            {
                bool isOldPersonActive = (oldPerson.Status.Id == (int)PersonStatusType.Active || oldPerson.Status.Id == (int)PersonStatusType.TerminationPending);

                if (personStatusId == (int)PersonStatusType.Terminated && isOldPersonActive)
                {
                    if (person.TerminationDate != null)
                    {
                        MailUtil.SendDeactivateAccountEmail(person.FirstName, person.LastName, person.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    }
                }
                if ((personStatusId == (int)PersonStatusType.Terminated || personStatusId == (int)PersonStatusType.TerminationPending) && (oldPerson.Status.Id == (int)PersonStatusType.Active))
                {
                    SendReviewCancelationMail(person.Id.Value, userLogin);
                }
                if (isOldPersonActive && isPersonActive && oldPerson.HireDate.Date != person.HireDate)
                {
                    SendHireDateChangedEmail(oldPerson, person, loginPageUrl);
                }
            }
            if (isRehireDueToPay)
            {
                //deactivate account and Activate account.
                var terminationDate = person.EmploymentHistory.Last(p => p.HireDate.Date < person.HireDate.Date).TerminationDate;
                if (terminationDate != null)
                    MailUtil.SendDeactivateAccountEmail(person.FirstName, person.LastName, terminationDate.Value.ToString(Constants.Formatting.EntryDateFormat));
            }
            if (isPersonActive)//rehiring due to pay or adding person or rehiring normally
            {
                SendActivateAccountEmail(person, oldPerson, loginPageUrl);
            }
            if (oldPerson != null && (oldPerson.CohortAssignment.Id != person.CohortAssignment.Id))
            {
                MailUtil.SendCohortAssignmentChangeEmail(person.FirstName + " " + person.LastName, oldPerson.CohortAssignment.Name, person.CohortAssignment.Name);
            }
        }

        //public void SendCompensationChangeEmail(Person person, Pay oldPay, Pay newPay, bool isRehire)
        //{
        //    if (person != null)
        //    {
        //        if (isRehire)
        //        {
        //            MailUtil.SendCompensationChangeRehireEmail(person.FirstName, person.LastName, newPay.StartDate.Date.ToString(Constants.Formatting.EntryDateFormat), person.Alias, newPay.HtmlEncodedTitleName, person.TelephoneNumber, person.IsOffshore, person.Manager.FirstName+" "+person.Manager.LastName, newPay.HtmlEncodedDivisionName, Generic.GetDescription(oldPay.Timescale), Generic.GetDescription(newPay.Timescale));
        //        }
        //        else if ((oldPay.Timescale != newPay.Timescale))
        //        {
        //            string effective = newPay.EndDate.HasValue && newPay.EndDate.Value.Date != Constants.Dates.FutureDate.Date ? newPay.StartDate.Date.ToString(Constants.Formatting.EntryDateFormat) + " to " + newPay.EndDate.Value.Date.AddDays(-1).ToString(Constants.Formatting.EntryDateFormat) : newPay.StartDate.Date.ToString(Constants.Formatting.EntryDateFormat);
        //            MailUtil.SendCompensationChangeEmail(person.FirstName, person.LastName, Generic.GetDescription(oldPay.Timescale), Generic.GetDescription(newPay.Timescale), effective);
        //        }
        //    }
        //}

        public static void SendReviewCancelationMail(int personId, string userLogin)
        {
            var feedbacks = ProjectDAL.GetPersonsForProjectReviewCanceled(personId, userLogin);
            if (feedbacks.Count > 0)
            {
                MailUtil.SendReviewCanceledMailNotification(feedbacks);
            }
        }

        /// <summary>
        /// Sends administrator added email.
        /// </summary>
        /// <param name="person">person's current data.</param>
        /// <param name="oldPerson">person's old data.</param>
        public void SendAdministratorAddedEmail(Person person, Person oldPerson)
        {
            if (!person.RoleNames.Contains(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) && person.Seniority.Name != DataTransferObjects.Constants.SeniorityNames.AdminiSeniorityName) return;
            bool isOldPersonAdministrator = (oldPerson != null) && oldPerson.RoleNames.Contains(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            bool isPersonAdminSeniority = person.Seniority.Name == DataTransferObjects.Constants.SeniorityNames.AdminiSeniorityName;
            bool isOldPersonAdminSeniority = (oldPerson != null) && oldPerson.Seniority.Name == DataTransferObjects.Constants.SeniorityNames.AdminiSeniorityName;
            if (!isOldPersonAdministrator || (oldPerson.Status.Id == (int)PersonStatusType.Terminated && person.Status.Id != (int)PersonStatusType.Terminated) || (!isOldPersonAdminSeniority && isPersonAdminSeniority))
            {
                MailUtil.SendAdministratorAddedEmail(person.FirstName, person.LastName);
            }
        }

        /// <summary>
        /// Sends activate account email.
        /// </summary>
        /// <param name="person">Person,to whom we need to send email.</param>
        /// <param name="loginPageUrl">Login page url of site.</param>
        private static void SendActivateAccountEmail(Person person, Person oldPerson, string loginPageUrl)
        {
            bool isOldPersonContingentOrTerminated = (oldPerson == null) || (oldPerson.Status.Id == (int)PersonStatusType.Contingent || oldPerson.Status.Id == (int)PersonStatusType.Terminated);
            if (!isOldPersonContingentOrTerminated) return;
            MailUtil.SendActivateAccountEmail(person.FirstName, person.LastName, person.HireDate.ToString(Constants.Formatting.EntryDateFormat),
                                              person.Alias, (person.Title != null) ? person.Title.TitleName : null, (person.CurrentPay != null) ? Generic.GetDescription(person.CurrentPay.Timescale) : null, person.TelephoneNumber,
                                              person.IsOffshore ? "Yes" : "No", person.ManagerName, person.DivisionType.ToString());

            DateTime currentPmTime = SettingsHelper.GetCurrentPMTime();
            TimeSpan welcomeEmailTimeStamp = new TimeSpan(7, 0, 0);
            if (person.HireDate.Date >= currentPmTime.Date &&
                (person.HireDate.Date != currentPmTime.Date || currentPmTime.TimeOfDay <= welcomeEmailTimeStamp))
                return;
            //send welcome mail if person have past hire date
            var companyName = ConfigurationDAL.GetCompanyName();
            SendWelcomeEmail(person, companyName, loginPageUrl);
        }

        /// <summary>
        /// Sends hire date changed email.
        /// </summary>
        /// <param name="oldPerson">Old person data,to whom we need to send email.</param>
        /// <param name="person">Person,to whom we need to send email</param>
        /// <param name="loginPageUrl">Login page url of site.</param>
        private static void SendHireDateChangedEmail(Person oldPerson, Person person, string loginPageUrl)
        {
            MailUtil.SendHireDateChangedEmail(person.FirstName, person.LastName, oldPerson.HireDate.ToString(Constants.Formatting.EntryDateFormat),
                        person.HireDate.ToString(Constants.Formatting.EntryDateFormat), person.Alias, (person.Title != null) ? person.Title.TitleName : null,
                        (person.CurrentPay != null) ? Generic.GetDescription(person.CurrentPay.Timescale) : null, person.TelephoneNumber);
            DateTime currentPmTime = SettingsHelper.GetCurrentPMTime();
            TimeSpan welcomeEmailTimeStamp = new TimeSpan(7, 0, 0);
            if (person.HireDate.Date > currentPmTime.Date || (person.HireDate.Date == currentPmTime.Date && currentPmTime.TimeOfDay < welcomeEmailTimeStamp))
            {
                //lockout the user
                AspMembershipDAL.UserSetLockedOut(person.Alias, Membership.ApplicationName);
            }
            else if (oldPerson.HireDate.Date > currentPmTime.Date && (person.HireDate.Date < currentPmTime.Date || (person.HireDate.Date == currentPmTime.Date && currentPmTime.TimeOfDay > welcomeEmailTimeStamp)))
            {
                //send welcome mail if person have past hire date
                var companyName = ConfigurationDAL.GetCompanyName();
                SendWelcomeEmail(person, companyName, loginPageUrl);
            }
        }

        /// <summary>
        /// Stores all data into the database and process the notification.
        /// </summary>
        /// <param name="person">The data to be stored.</param>
        /// <param name="currentUser">A currently logged user.</param>
        /// <param name="oldPerson">Old person data.</param>
        private static void ProcessPersonData(Person person, string currentUser, Person oldPerson, string loginPageUrl, bool saveCurrentPay, string userLogin)
        {
            bool isReHireDueToPay = false;
            bool isAdministrator = Roles.IsUserInRole(currentUser, DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, "TR_ProcessPersonData");

                if (!person.Id.HasValue)
                {
                    if (!string.IsNullOrEmpty(person.Alias))
                    {
                        // Create a login
                        string password = Membership.GeneratePassword(Math.Max(Membership.MinRequiredPasswordLength, 1),
                                                                      Membership.MinRequiredNonAlphanumericCharacters);
                        string salt = GenerateSalt();
                        string hashedPassword = EncodePassword(password, salt);

                        PersonDAL.Createuser(person.Alias, hashedPassword, salt, person.Alias, connection, transaction);
                    }
                    // Create a Person record
                    PersonDAL.PersonInsert(person, currentUser, connection, transaction);
                }
                else
                {
                    if (string.Compare(oldPerson.Alias, person.Alias, true) != 0)
                    {
                        if (Membership.FindUsersByName(person.Alias).Count == 0)
                            PersonDAL.MembershipAliasUpdate(oldPerson.Alias, person.Alias, connection, transaction);
                        else
                            throw new Exception("There is another Person with the same Email.");
                    }

                    //PersonDAL.PersonSetStatus(person);
                    PersonDAL.PersonUpdate(person, currentUser, connection, transaction);
                }

                bool isLockedOutUpdated = false;

                // Locking the users:the rule is the person should be in lock out state from termination date to midnight of next hiredate.
                //If person is rehired also we will lock the person and unlock him after sending the welcome mail i.e. on the new hiredate.
                if (oldPerson != null && oldPerson.Status != null && person.Status != null &&
                    oldPerson.Status.Id != person.Status.Id &&
                    (person.Status.Id == (int)PersonStatusType.Terminated || oldPerson.Status.Id == (int)PersonStatusType.Terminated))
                {
                    AspMembershipDAL.UserSetLockedOut(person.Alias, Membership.ApplicationName, connection, transaction);
                    person.LockedOut = isLockedOutUpdated = true;
                }

                // Saving the Locked-Out value
                if (isAdministrator  // && userInfo != null && person.LockedOut != userInfo.IsLockedOut
                     && !isLockedOutUpdated)
                {
                    if (person.LockedOut)
                    {
                        AspMembershipDAL.UserSetLockedOut(person.Alias, Membership.ApplicationName, connection, transaction);
                    }
                    else
                    {
                        AspMembershipDAL.UserUnLockOut(person.Alias, Membership.ApplicationName, connection, transaction);
                    }
                }

                transaction.Commit();

                // Saving the person's payment info

                if (saveCurrentPay && person.CurrentPay != null && person.Id.HasValue)
                {
                    person.CurrentPay.PersonId = person.Id.Value;
                    isReHireDueToPay = PayDAL.SavePayDatail(person.CurrentPay, connection, null, currentUser);
                }
                if (isReHireDueToPay)
                {
                    person.EmploymentHistory = PersonDAL.GetPersonEmploymentHistoryById(person.Id.Value);
                }
                if (person.Id != null) person.CurrentPay = PayDAL.GetCurrentByPerson(person.Id.Value);
            }
            SendMailsAfterProcessPersonData(oldPerson, person, isReHireDueToPay, loginPageUrl, userLogin);
        }

        /// <summary>
        /// Person Insert/Update DB validations are done by this Method.
        /// </summary>
        /// <param name="person"></param>
        public void PersonValidations(Person person)
        {
            PersonDAL.PersonValidations(person);
        }

        /// <summary>
        /// Retrives a list of overheads for the specified <see cref="Timescale"/>.
        /// </summary>
        /// <param name="timescale">The <see cref="Timescale"/> to retrive the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public List<PersonOverhead> GetPersonOverheadByTimescale(TimescaleType timescale, DateTime? effectiveDate)
        {
            return PersonDAL.PersonOverheadListByTimescale(timescale, effectiveDate);
        }

        /// <summary>
        /// Retrives the person's rate.
        /// </summary>
        /// <param name="milestonePerson">The milestone-person association to the rate be calculated for.</param>
        /// <returns>The <see cref="MilestonePerson"/> object with calculated data.</returns>
        public MilestonePerson GetPersonRate(MilestonePerson milestonePerson)
        {
            return PersonRateCalculator.CalculateRate(milestonePerson, null);
        }

        /// <summary>
        /// Calculates the person's rate.
        /// </summary>
        /// <param name="person">A <see cref="Person"/> object to calculate the data for.</param>
        /// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
        /// <param name="proposedRate">A proposed person's hourly rate.</param>
        /// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
        public ComputedFinancialsEx CalculateProposedFinancialsPerson(Person person, decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount, bool isMarginTestPage, DateTime? effectiveDate)
        {
            PersonRateCalculator calculator = GetCalculatorForProposedFinancials(person, proposedRate, proposedHoursPerWeek, isMarginTestPage, effectiveDate);

            return calculator.CalculateProposedFinancials(proposedRate, proposedHoursPerWeek, clientDiscount, effectiveDate);
        }

        private static PersonRateCalculator GetCalculatorForProposedFinancials(Person person, decimal proposedRate, decimal proposedHoursPerWeek, bool isMarginTestPage, DateTime? effectiveDate)
        {
            PersonRateCalculator calculator = new PersonRateCalculator(person, isMarginTestPage, effectiveDate);

            if (person.CurrentPay != null)
            {
                person.OverheadList = PersonDAL.PersonOverheadListByTimescale(person.CurrentPay.Timescale, effectiveDate);

                //Remove over MLF Over head if it has 0 rate
                foreach (var overhead in person.OverheadList.FindAll(OH => OH.IsMLF && OH.Rate == 0))
                {
                    person.OverheadList.Remove(overhead);
                }

                bool isHourlyAmount =
                   person.CurrentPay.Timescale == TimescaleType._1099Ctc ||
                   person.CurrentPay.Timescale == TimescaleType.Hourly ||
                   person.CurrentPay.Timescale == TimescaleType.PercRevenue;

                if (isHourlyAmount)
                    person.CurrentPay.AmountHourly = person.CurrentPay.Amount;
                else
                    person.CurrentPay.AmountHourly = person.CurrentPay.Amount / PersonRateCalculator.WorkingHoursInYear(calculator.DaysInYear, proposedHoursPerWeek);

                //  Update hourly rate for percent of revenue persons with proposed rate
                if (person.CurrentPay.Timescale == TimescaleType.PercRevenue)
                {
                    person.CurrentPay.AmountHourly *= decimal.Multiply(proposedRate, 0.01M);
                }

                foreach (PersonOverhead overhead in person.OverheadList.Where(overhead => overhead.RateType != null))
                {
                    switch (overhead.RateType.Id)
                    {
                        case (int)OverheadRateTypes.BillRateMultiplier:
                            overhead.HourlyValue = overhead.BillRateMultiplier * proposedRate / 100M;
                            break;

                        case (int)OverheadRateTypes.PayRateMultiplier:
                            overhead.HourlyValue = overhead.Rate * person.CurrentPay.AmountHourly / 100M;
                            break;

                        case (int)OverheadRateTypes.MonthlyCost:
                            var hoursPerYear = PersonRateCalculator.WorkingHoursInYear(calculator.DaysInYear, (int)proposedHoursPerWeek);
                            overhead.HourlyValue = overhead.Rate * 12 / hoursPerYear;
                            break;

                        default:
                            overhead.HourlyValue = overhead.Rate;
                            break;
                    }
                }
            }
            else
            {
                person.OverheadList = new List<PersonOverhead>();
            }

            person.OverheadList.Add(calculator.CalculateBonusOverhead(proposedHoursPerWeek));
            return calculator;
        }

        /// <summary>
        /// Saves a payment data.
        /// </summary>
        /// <param name="pay">The <see cref="Pay"/> object to be saved.</param>
        /// <param name="loginPageUrl">Login page url of site.</param>
        /// <param name="user">Current login user.</param>
        public void SavePay(Pay pay, string loginPageUrl, string user = null)
        {
            bool isRehireDueToPay = PayDAL.SavePayDatail(pay, null, null, user);
            if (!isRehireDueToPay) return;
            var person = GetPersonById(pay.PersonId);
            if (person.Id != null)
            {
                person.CurrentPay = PayDAL.GetCurrentByPerson(person.Id.Value);
                person.EmploymentHistory = PersonDAL.GetPersonEmploymentHistoryById(person.Id.Value);
            }
            person.RoleNames = Roles.GetRolesForUser(person.Alias);
            SendMailsAfterProcessPersonData(null, person, true, loginPageUrl, user);
        }

        /// <summary>
        /// Deletes the pay with given person and start date.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="startDate"></param>
        public void DeletePay(int personId, DateTime startDate)
        {
            PayDAL.DeletePay(personId, startDate);
        }

        /// <summary>
        /// Selects a list of the seniorities.
        /// </summary>
        /// <returns>A list of the <see cref="Seniority"/> objects.</returns>
        public List<Seniority> ListSeniorities()
        {
            return SeniorityDAL.ListAll();
        }

        /// <summary>
        /// Sets permissions for user
        /// </summary>
        /// <param name="person">Person to set permissions for</param>
        /// <param name="permissions">Permissions to set for the user</param>
        public void SetPermissionsForPerson(Person person, PersonPermission permissions)
        {
            PersonDAL.SetPermissionsForPerson(person, permissions);
        }

        /// <summary>
        /// Check's if there's compensation record covering milestone
        /// See #886 for details.
        /// </summary>
        /// <param name="person">Person to check against</param>
        /// <returns>True if there's such record, false otherwise</returns>
        public bool IsCompensationCoversMilestone(Person person, DateTime? start, DateTime? end)
        {
            return PersonDAL.IsCompensationCoversMilestone(person, start, end);
        }

        /// <summary>
        /// Verifies whether a user has compensation at this moment
        /// </summary>
        /// <param name="personId">Id of the person</param>
        /// <returns>True if a person has active compensation, false otherwise</returns>
        public bool CurrentPayExists(int personId)
        {
            return PersonDAL.CurrentPayExists(personId);
        }

        /// <summary>
        /// Saves USer Temporary Credentials
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="PMLoginPageUrl"></param>
        /// <param name="PMChangePasswordPageUrl"></param>
        /// <returns></returns>
        public bool SaveUserTemporaryCredentials(string userName, string PMLoginPageUrl, string PMChangePasswordPageUrl)
        {
            string password = Membership.GeneratePassword(Math.Max(Membership.MinRequiredPasswordLength, 1),
                                                                        Membership.MinRequiredNonAlphanumericCharacters);
            string salt = GenerateSalt();
            string hashedPassword = EncodePassword(password, salt);
            var emailTemplate = EmailTemplateDAL.EmailTemplateGetByName(Resources.Messages.PasswordResetEmailTemplateName);
            try
            {
                bool result = PersonDAL.UserTemporaryCredentialsInsert(userName, hashedPassword, 1, salt);
                if (result)
                {
                    MailUtil.SendForgotPasswordNotification(userName, password, emailTemplate, PMLoginPageUrl, PMChangePasswordPageUrl);
                }
                return result;
            }
            catch (Exception e)
            {
                PersonDAL.DeleteTemporaryCredentialsByUserName(userName);
                throw (e);
            }
        }

        /// <summary>
        /// Check weather temporary credentials are valid or not.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckIfTemporaryCredentialsValid(string userName, string password)
        {
            var userCredentials = PersonDAL.GetTemporaryCredentialsByUserName(userName);
            if (userCredentials != null)
            {
                string hashedPassword = EncodePassword(password, userCredentials.PasswordSalt);
                bool result = (hashedPassword == userCredentials.Password);
                return result;
            }
            return false;
        }

        /// <summary>
        /// Sets New password for the given user with given new password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="newPassword"></param>
        public void SetNewPasswordForUser(string userName, string newPassword)
        {
            string salt = GenerateSalt();
            string hashedPassword = EncodePassword(newPassword, salt);
            PersonDAL.SetNewPasswordForUser(userName, hashedPassword, salt, 1, DateTime.UtcNow);
            PersonDAL.DeleteTemporaryCredentialsByUserName(userName);
        }

        public List<Person> PersonListByCategoryTypeAndPeriod(BudgetCategoryType categoryType, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.PersonListByCategoryTypeAndPeriod(categoryType, startDate, endDate);
        }

        public bool CheckPersonTimeEntriesAfterTerminationDate(int personId, DateTime terminationDate)
        {
            return TimeEntryDAL.CheckPersonTimeEntriesAfterTerminationDate(personId, terminationDate);
        }

        public bool CheckPersonTimeEntriesAfterHireDate(int personId)
        {
            return TimeEntryDAL.CheckPersonTimeEntriesAfterHireDate(personId);
        }

        public Owner CheckIfPersonStatusCanChangeFromActiveToContingent(int personId)
        {
            return PersonDAL.CheckIfPersonStatusCanChangeFromActiveToContingent(personId);
        }
        public List<Milestone> GetPersonMilestonesAfterTerminationDate(int personId, DateTime terminationDate)
        {
            return MilestoneDAL.GetPersonMilestonesAfterTerminationDate(personId, terminationDate);
        }

        public List<Project> GetOwnerProjectsAfterTerminationDate(int personId, DateTime terminationDate)
        {
            return ProjectDAL.GetOwnerProjectsAfterTerminationDate(personId, terminationDate);
        }

        public List<Opportunity> GetActiveOpportunitiesByOwnerId(int personId)
        {
            return OpportunityDAL.GetActiveOpportunitiesByOwnerId(personId);
        }

        public List<UserPasswordsHistory> GetPasswordHistoryByUserName(string userName)
        {
            return PersonDAL.GetPasswordHistoryByUserName(userName);
        }

        public string GetEncodedPassword(string password, string passwordSalt)
        {
            return EncodePassword(password, passwordSalt);
        }

        public void RestartCustomMembershipProvider()
        {
            System.Web.HttpRuntime.UnloadAppDomain();
        }

        public void SendLockedOutNotificationEmail(string userName, string loginPageUrl)
        {
            MailUtil.SendLockedOutNotificationEmail(userName, loginPageUrl);
        }

        public Dictionary<DateTime, bool> GetIsNoteRequiredDetailsForSelectedDateRange(DateTime start, DateTime end, int personId)
        {
            return PersonDAL.GetIsNoteRequiredDetailsForSelectedDateRange(start, end, personId);
        }

        /// <summary>
        /// Saves the given straw man and with the given pay.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="currentPay"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public int? SaveStrawman(Person person, Pay currentPay, string userLogin)
        {
            PersonDAL.SaveStrawMan(person, currentPay, userLogin);
            return person.Id;
        }

        /// <summary>
        /// deletes the given straw man.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="userLogin"></param>
        public void DeleteStrawman(int personId, string userLogin)
        {
            PersonDAL.DeleteStrawman(personId, userLogin);
        }

        /// <summary>
        /// saves straw man from the given existing straw man.
        /// </summary>
        /// <param name="existingPersonId"></param>
        /// <param name="person"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public int SaveStrawManFromExisting(int existingPersonId, Person person, string userLogin)
        {
            int newPersonId;
            PersonDAL.SaveStrawManFromExisting(existingPersonId, person, out newPersonId, userLogin);
            return newPersonId;
        }

        /// <summary>
        /// gets straw man details by given Id.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Person GetStrawmanDetailsById(int personId)
        {
            var person = PersonDAL.GetPersonFirstLastNameById(personId);
            person.PaymentHistory = PayDAL.GetHistoryByPerson(personId);
            return person;
        }

        /// <summary>
        /// gets the person details In short.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Person GetPersonDetailsShort(int personId)
        {
            return PersonDAL.GetPersonFirstLastNameById(personId);
        }

        public Person GetPayHistoryShortByPerson(int personId)
        {
            var person = new Person
            {
                Id = personId,
                PaymentHistory = PayDAL.GetPayHistoryShortByPerson(personId)
            };
            return person;
        }

        public List<Person> GetStrawmenListAll()
        {
            return PersonDAL.GetStrawmenListAll();
        }

        public List<Person> GetStrawmenListAllShort(bool includeInactive)
        {
            return PersonDAL.GetStrawmenListAllShort(includeInactive);
        }

        public Person GetStrawmanDetailsByIdWithCurrentPay(int id)
        {
            return PersonDAL.GetStrawmanDetailsByIdWithCurrentPay(id);
        }

        public List<ConsultantDemandItem> GetConsultantswithDemand(DateTime startDate, DateTime endDate)
        {
            return PersonDAL.GetConsultantswithDemand(startDate, endDate);
        }

        public bool IsPersonHaveActiveStatusDuringThisPeriod(int personId, DateTime startDate, DateTime? endDate)
        {
            return PersonDAL.IsPersonHaveActiveStatusDuringThisPeriod(personId, startDate, endDate);
        }

        public List<Person> PersonsListHavingActiveStatusDuringThisPeriod(DateTime startDate, DateTime endDate)
        {
            return PersonDAL.PersonsListHavingActiveStatusDuringThisPeriod(startDate, endDate);
        }

        public List<Person> GetApprovedByManagerList()
        {
            return PersonDAL.GetApprovedByManagerList();
        }

        public List<Person> GetPersonListBySearchKeyword(String looked)
        {
            return PersonDAL.GetPersonListBySearchKeyword(looked);
        }

        public List<Triple<DateTime, bool, bool>> IsPersonSalaryTypeListByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return PayDAL.IsPersonSalaryTypeListByPeriod(personId, startDate, endDate);
        }

        public List<Pay> GetHistoryByPerson(int personId)
        {
            return PayDAL.GetHistoryByPerson(personId);
        }

        public List<Person> GetStrawmanListShortFilterWithTodayPay()
        {
            return PersonDAL.GetStrawmanListShortFilterWithTodayPay();
        }

        public List<TerminationReason> GetTerminationReasonsList()
        {
            return PersonDAL.GetTerminationReasonsList();
        }

        public Person GetPersonHireAndTerminationDate(int personId)
        {
            Person result = PersonDAL.GetPersonHireAndTerminationDateById(personId);

            return result;
        }

        /// <summary>
        /// Selects a list of the seniority Categories.
        /// </summary>
        /// <returns>A list of the <see cref="Seniority"/> objects.</returns>
        public List<SeniorityCategory> ListAllSeniorityCategories()
        {
            return SeniorityDAL.ListAllSeniorityCategories();
        }

        public List<Person> GetPersonListWithRole(string rolename)
        {
            return PersonDAL.GetPersonListWithRole(rolename);
        }

        public List<TimeTypeRecord> GetPersonAdministrativeTimeTypesInRange(int personId, DateTime startDate, DateTime endDate, bool includePTO, bool includeHoliday, bool includeUnpaid, bool inludeSickLeave)
        {
            return PersonDAL.GetPersonAdministrativeTimeTypesInRange(personId, startDate, endDate, includePTO, includeHoliday, includeUnpaid, inludeSickLeave);
        }

        public bool IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale(int personId, DateTime startDate, DateTime endDate, int timeScaleId)
        {
            return PersonDAL.IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale(personId, startDate, endDate, timeScaleId);
        }

        public PersonPassword GetPersonEncodedPassword(int personId)
        {
            return PersonDAL.GetPersonEncodedPassword(personId);
        }

        public void DeletePersonEncodedPassword(int personId)
        {
            PersonDAL.DeletePersonEncodedPassword(personId);
        }

        private static string DecodePasswordWithoutHash(string encryptpwd)
        {
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] decodedBytes = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(decodedBytes, 0, decodedBytes.Length);
            char[] decodedCharacters = new char[charCount];
            Decode.GetChars(decodedBytes, 0, decodedBytes.Length, decodedCharacters, 0);
            string decodedPassword = new String(decodedCharacters);
            return decodedPassword;
        }

        public bool CheckIfPersonPasswordValid(string alias, string password)
        {
            int? personId = PersonDAL.PersonGetByAlias(alias).Id;
            if (personId != null)
            {
                var personCredentials = PersonDAL.GetPersonEncodedPassword(personId.Value);
                if (personCredentials != null)
                {
                    bool result = (password == DecodePasswordWithoutHash(personCredentials.Password));
                    return result;
                }
            }
            return false;
        }

        public void UpdateUserPassword(int personId, string userName, string newPassword)
        {
            string salt = GenerateSalt();
            string hashedPassword = EncodePassword(newPassword, salt);
            PersonDAL.SetNewPasswordForUser(userName, hashedPassword, salt, 1, DateTime.UtcNow);
            PersonDAL.DeletePersonEncodedPassword(personId);
        }

        public Pay GetCurrentByPerson(int personId)
        {
            return PayDAL.GetCurrentByPerson(personId);
        }

        public List<Person> GetActivePersonsByProjectId(int projectId)
        {
            return PersonDAL.GetActivePersonsByProjectId(projectId);
        }

        public Title GetPersonTitleByRange(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.GetPersonTitleByRange(personId, startDate, endDate);
        }

        public bool CheckIfRangeWithinHireAndTermination(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfRangeWithinHireAndTermination(personId, startDate, endDate);
        }

        public bool CheckIfPersonConsultantTypeInAPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfPersonConsultantTypeInAPeriod(personId, startDate, endDate);
        }

        public List<Project> GetCommissionsValidationByPersonId(int personId, DateTime hireDate,
                                                                       DateTime? terminationDate, int personStatusId,
                                                                       int divisionId, bool IsReHire)
        {
            return PersonDAL.GetCommissionsValidationByPersonId(personId, hireDate, terminationDate, personStatusId, divisionId, IsReHire);
        }

        public Person CheckIfValidDivision(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfValidDivision(personId, startDate, endDate);
        }

        public bool CheckIfPersonEntriesOverlapps(int milestoneId, int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfPersonEntriesOverlapps(milestoneId, personId, startDate, endDate);
        }

        public List<Person> GetPersonsByPayTypesAndByStatusIds(string statusIds, string payTypeIds)
        {
            return PersonDAL.GetPersonsByPayTypesAndByStatusIds(statusIds, payTypeIds);
        }

        public List<CohortAssignment> GetAllCohortAssignments()
        {
            return PersonDAL.GetAllCohortAssignments();
        }

        public List<Person> GetPTOReport(DateTime startDate, DateTime endDate, bool includeCompanyHolidays)
        {
            return PersonDAL.GetPTOReport(startDate, endDate, includeCompanyHolidays);
        }

        public List<MSBadge> GetBadgeDetailsByPersonId(int personId)
        {
            return PersonDAL.GetBadgeDetailsByPersonId(personId);
        }

        public List<MSBadge> GetLogic2020BadgeHistory(int personId)
        {
            return PersonDAL.GetLogic2020BadgeHistory(personId);
        }

        public void SaveBadgeDetailsByPersonId(MSBadge msBadge)
        {
            PersonDAL.SaveBadgeDetailsByPersonId(msBadge);
        }

        public void UpdateMSBadgeDetailsByPersonId(int personId, int updatedBy)
        {
            PersonDAL.UpdateMSBadgeDetailsByPersonId(personId, updatedBy);
        }

        public List<bool> CheckIfDatesInDeactivationHistory(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfDatesInDeactivationHistory(personId, startDate, endDate);
        }

        public bool CheckIfPersonInProjectForDates(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.CheckIfPersonInProjectForDates(personId, startDate, endDate);
        }

        public bool CheckIfPersonIsRestrictedByProjectId(int personId, int projectId, DateTime chargeDate)
        {
            return PersonDAL.CheckIfPersonIsRestrictedByProjectId(personId, projectId, chargeDate);
        }

        public PersonBadgeHistories GetBadgeHistoryByPersonId(int personId)
        {
            return PersonDAL.GetBadgeHistoryByPersonId(personId);
        }

        public bool CheckIfPersonInProjectsForThisPeriod(DateTime? modifiedEndDate, DateTime? oldEndDate, DateTime? modifiedStartDate, DateTime? oldStartDate, int personId)
        {
            return PersonDAL.CheckIfPersonInProjectsForThisPeriod(modifiedEndDate, oldEndDate, modifiedStartDate, oldStartDate, personId);
        }

        public List<MSBadge> GetBadgeRecordsAfterDeactivatedDate(int personId, DateTime deactivatedDate)
        {
            return PersonDAL.GetBadgeRecordsAfterDeactivatedDate(personId, deactivatedDate);
        }

        public List<MSBadge> GetBadgeRecordsByProjectId(int projectId)
        {
            return PersonDAL.GetBadgeRecordsByProjectId(projectId);
        }

        public bool IsPersonSalaryTypeInGivenRange(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.IsPersonSalaryTypeInGivenRange(personId, startDate, endDate);
        }


        public List<Person> GetPracticeLeaderships(int? divisionId)
        {
            return PersonDAL.GetPracticeLeaderships(divisionId);
        }

        public List<PersonDivision> GetPersonDivisions()
        {
            return PersonDAL.GetPersonDivisions();
        }

        public PersonDivision GetPersonDivisionById(int divisioId)
        {
            return PersonDAL.GetPersonDivisionById(divisioId);
        }

        public void UpdatePersonDivision(PersonDivision division)
        {
            PersonDAL.UpdatePersonDivision(division);
        }

        public List<Owner> CheckIfPersonIsOwnerForDivisionAndOrPractice(int personId)
        {
            return PersonDAL.CheckIfPersonIsOwnerForDivisionAndOrPractice(personId);
        }

        public void SaveReportFilterValues(int currentUserId, int reportId, string data, int previousUserId, string sessionId)
        {
            PersonDAL.SaveReportFilterValues(currentUserId, reportId, data, previousUserId, sessionId);
        }

        public string GetReportFilterValues(int currentUserId, int reportId, int previousUserId, string sessionId)
        {
            return PersonDAL.GetReportFilterValues(currentUserId, reportId, previousUserId, sessionId);
        }

        public void DeleteReportFilterValues(int currentUserId, int previousUserId, string sessionId)
        {
            PersonDAL.DeleteReportFilterValues(currentUserId, previousUserId, sessionId);
        }

        public List<ConsultantPTOHours> GetConsultantPTOEntries(DateTime startDate, DateTime endDate, int step, bool includeActivePersons, bool includeContingentPersons, bool isW2Salary, bool isW2Hourly, string practiceIds, string divisionIds, string titleIds, int sortId, string sortDirection)
        {
            return PersonDAL.GetConsultantPTOEntries(startDate, endDate, step, includeActivePersons, includeContingentPersons, isW2Salary, isW2Hourly, practiceIds, divisionIds, titleIds, sortId, sortDirection);
        }
        #endregion IPersonService Members
    }
}

