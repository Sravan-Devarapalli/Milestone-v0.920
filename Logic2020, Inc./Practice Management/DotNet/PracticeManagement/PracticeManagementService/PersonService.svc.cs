﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Activation;
using System.Transactions;
using System.Web.Security;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;
using System.IO;
using DataTransferObjects.ContextObjects;
using System.Text;
using System.Security.Cryptography;
using System.Web.Configuration;
using PraticeManagement;
using System.Collections.Specialized;

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
        public void SetAsDefaultManager(Person person)
        {
            PersonDAL.SetAsDefaultManager(person);
        }

        /// <summary>
        /// Checks if the person is a manager to somebody
        /// </summary>
        public bool IsSomeonesManager(Person person)
        {
            return PersonDAL.IsSomeonesManager(person);
        }

        /// <summary>
        /// Lists managers subordinates
        /// </summary>
        /// <param name="person">Manager</param>
        /// <returns>List of subordinates</returns>
        public Person[] ListManagersSubordinates(Person person)
        {
            return PersonDAL.ListManagersSubordinates(person);
        }

        /// <summary>
        /// Sets the new manager for everybody who
        /// </summary>
        public void SetNewManager(Person oldManager, Person newManager)
        {
            PersonDAL.SetNewManager(oldManager, newManager);
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public DataSet GetConsultantUtilizationReport(ConsultantTableReportContext context)
        {
            return PersonDAL.GetConsultantUtilizationReport(context);
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public List<Quadruple<Person, int[], int, int>> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context)
        {
            return PersonDAL.GetConsultantUtilizationWeekly(context);
        }

        public List<Triple<Person, int[], int>> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context)
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

        public List<Person> GetPersonListWithCurrentPay(
            int? practice,
            bool active,
            int pageSize,
            int pageNo,
            string looked,
            int? recruiterId,
            string userName,
            string sortBy,
            int? timeScaleId,
            bool projected,
            bool terminated,
            bool inactive,
            char? alphabet)
        {
            PersonRateCalculator.VerifyPrivileges(userName, ref recruiterId);
            return
                PersonDAL.PersonListFilteredWithCurrentPay(practice, !active, pageSize, pageNo, looked, DateTime.MinValue, DateTime.MinValue, recruiterId, null, sortBy, timeScaleId, projected, terminated, inactive, alphabet);
        }

        public List<Person> GetPersonListWithCurrentPayByCommaSeparatedIdsList(
            string practiceIdsSelected,
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
                PersonDAL.PersonListFilteredWithCurrentPayByCommaSeparatedIdsList(practiceIdsSelected, !active, pageSize, pageNo, looked, DateTime.MinValue, DateTime.MinValue, recruiterIdsSelected, null, sortBy, timeScaleIdsSelected, projected, terminated, terminatedPending, alphabet);
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
        /// <param name="practice">The <see cref="Person"/>'s default practice.</param>
        /// <param name="showAll">List all <see cref="Person"/>s if true and the only active otherwise.</param>
        /// <param name="looked">List all <see cref="Person"/>s by search string that matches for first name or last name  .</param>
        /// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
        /// <param name="userName">A current user.</param>
        /// <returns>The number of the persons those match with the specified conditions.</returns>
        public int GetPersonCount(int? practice, bool active, string looked, int? recruiterId, string userName, int? timeScaleId, bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            PersonRateCalculator.VerifyPrivileges(userName, ref recruiterId);
            return PersonDAL.PersonGetCount(practice, !active, looked, recruiterId, timeScaleId, projected, terminated, terminatedPending, alphabet);
        }


        public int GetPersonCountByCommaSeperatedIdsList(string practiceIds, bool active, string looked, string recruiterIds, string userName, string timeScaleIds, bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            PersonRateCalculator.VerifyPrivileges(userName, ref recruiterIds);
            return PersonDAL.PersonGetCount(practiceIds, !active, looked, recruiterIds, timeScaleIds, projected, terminated, terminatedPending, alphabet);
        }

        /// <summary>
        /// Calculates a number of <see cref="Person"/>s working days in period.
        /// </summary>
        /// <param name="personId">Id of the person to get</param>
        /// <param name="startDate">mileStone start date </param>
        /// <param name="endDate">mileStone end date</param>
        /// <returns>The number of the persons working days in period.</returns>
        public int GetPersonWorkDaysNumber(int personId, DateTime startDate, DateTime endDate)
        {
            return PersonDAL.PersonWorkDaysNumber(personId, startDate, endDate);
        }

        /// <summary>
        /// Lists all active persons who receive some recruiter commissions.
        /// </summary>
        /// <returns>The list of <see cref="Person"/> objects.</returns>
        public List<Person> GetRecruiterList(int? personId, DateTime? hireDate)
        {
            return PersonDAL.PersonListRecruiter(personId, hireDate);
        }

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public List<Person> GetSalespersonList(bool includeInactive)
        {
            return PersonDAL.PersonListSalesperson(includeInactive);
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
        /// Retrieves all Heirarchi persons for a specified manager(Career Counselor).
        /// </summary>
        /// <param name="practiceManagerId">An ID of the manager(Counselor) to teh data be retrieved for.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public List<Person> GetCareerCounselorHierarchiPersons(int managerId)
        {
            return PersonDAL.GetCareerCounselorHierarchiPersons(managerId);//Here managerId is counselorId.
        }

        /// <summary>
        /// Read All persons firstname and last name  except having inactive status and must have compensation for today or in future.
        /// </summary>
        /// <param name="today"></param>
        /// <returns>List<Person></returns>
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

        // TODO: do we need this if we already have a list of all persons?
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
            Person result = PersonRateCalculator.GetPersonDetail(personId);
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
        public int SavePersonDetail(Person person, string currentUser, string loginPageUrl)
        {
            Person oldPerson = person.Id.HasValue ? PersonDAL.GetById(person.Id.Value) : null;

            bool isPreviouslyActive = person.Id.HasValue ? PersonDAL.IsPersonAlreadyHavingStatus((int)PersonStatusType.Active, person.Id) : true;

            try
            {
                ProcessPersonData(person, currentUser, oldPerson, loginPageUrl);

                if (oldPerson != null
                    && !oldPerson.IsWelcomeEmailSent
                    && oldPerson.Status.Id != (int)PersonStatusType.Active
                    && person.Status.Id == (int)PersonStatusType.Active
                    && !isPreviouslyActive)
                {
                    var companyName = ConfigurationDAL.GetCompanyName();
                    SendWelcomeEmail(person, companyName, loginPageUrl);
                }

                return person.Id.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendWelcomeEmail(Person person, string companyName, string loginPageUrl)
        {
            MembershipUser user = Membership.GetUser(person.Alias);
            if (user != null)
            {
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                string password = user.ResetPassword();
                MailUtil.SendWelcomeEmail(person.FirstName, person.Alias, password, companyName, loginPageUrl);
                PersonDAL.UpdateIsWelcomeEmailSentForPerson(person.Id);
            }
            else
            {
                throw new MembershipPasswordException();
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
            bRet = s.ComputeHash(bAll);


            return Convert.ToBase64String(bRet);
        }

        public static string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// Stores all data into the database and process the notification.
        /// </summary>
        /// <param name="person">The data to be stored.</param>
        /// <param name="currentUser">A currently logged user.</param>
        /// <param name="oldPerson">Old person data.</param>
        private static void ProcessPersonData(Person person, string currentUser, Person oldPerson, string loginPageUrl)
        {
            string password = string.Empty;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                if (!person.Id.HasValue)
                {


                    if (!string.IsNullOrEmpty(person.Alias))
                    {
                        // Create a login
                        password = Membership.GeneratePassword(Math.Max(Membership.MinRequiredPasswordLength, 1),
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

                // Locking the terminated users
                if (oldPerson != null && oldPerson.Status != null && person.Status != null &&
                    oldPerson.Status.Id != person.Status.Id &&
                    person.Status.Id == (int)PersonStatusType.Terminated)
                {
                    AspMembershipDAL.UserSetLockedOut(person.Alias, Membership.ApplicationName, connection, transaction);
                    person.LockedOut = isLockedOutUpdated = true;
                }

                // Saving the Locked-Out value
                if (Roles.IsUserInRole(currentUser, DataTransferObjects.Constants.RoleNames.AdministratorRoleName) // && userInfo != null && person.LockedOut != userInfo.IsLockedOut
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

                // Saving the recruiter commissions for the given person
                if (person.RecruiterCommission != null && person.Id.HasValue)
                {
                    List<RecruiterCommission> currentRecruiterCommission =
                        RecruiterCommissionDAL.DefaultRecruiterCommissionListByRecruitId(person.Id.Value);
                    foreach (RecruiterCommission commission in currentRecruiterCommission)
                    {
                        if (!person.RecruiterCommission.Exists(comm => comm.RecruiterId == commission.RecruiterId)
                            || person.RecruiterCommission.Count < 2 && currentRecruiterCommission.Count == 2)
                        {
                            RecruiterCommissionDAL.DeleteRecruiterCommissionDetail(commission, connection, transaction);
                        }
                    }
                    foreach (RecruiterCommission commission in person.RecruiterCommission)
                    {
                        commission.Recruit = person;
                        RecruiterCommissionDAL.SaveRecruiterCommissionDetail(commission, connection, transaction);
                    }

                }

                // Saving the person's payment info
                if (person.CurrentPay != null && person.Id.HasValue)
                {
                    person.CurrentPay.PersonId = person.Id.Value;
                    PayDAL.SavePayDatail(person.CurrentPay, connection, transaction);
                }

                // Saving the person's default commission info.
                if (person.DefaultPersonCommissions != null && person.Id.HasValue)
                {
                    foreach (DefaultCommission commission in person.DefaultPersonCommissions)
                    {
                        commission.PersonId = person.Id.Value;
                        if (commission.FractionOfMargin >= 0)
                        {
                            DefaultCommissionDAL.SaveDefaultCommissionDetail(commission, connection, transaction);
                        }
                        else
                        {
                            DefaultCommissionDAL.DeleteDefaultCommission(commission, connection, transaction);
                        }
                    }
                }

                //PersonDAL.PersonEnsureIntegrity(person.Id.Value, connection, transaction);

                transaction.Commit();
            }

            if (oldPerson == null && !string.IsNullOrEmpty(person.Alias) && person.Status.Id == (int)PersonStatusType.Active)
            {
                var companyName = ConfigurationDAL.GetCompanyName();
                MailUtil.SendWelcomeEmail(person.FirstName, person.Alias, password, companyName, loginPageUrl);
                PersonDAL.UpdateIsWelcomeEmailSentForPerson(person.Id);
            }

        }

        /// <summary>
        /// Retrives the list of the overheads for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the person to retrive the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public List<PersonOverhead> GetPersonOverheadByPerson(int personId)
        {
            return PersonDAL.PersonOverheadListByPerson(personId);
        }

        /// <summary>
        /// Retrives a list of overheads for the specified <see cref="Timescale"/>.
        /// </summary>
        /// <param name="timescale">The <see cref="Timescale"/> to retrive the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public List<PersonOverhead> GetPersonOverheadByTimescale(TimescaleType timescale)
        {
            return PersonDAL.PersonOverheadListByTimescale(timescale);
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
        /// <param name="personId">An ID of the <see cref="Person"/> to calculate the data for.</param>
        /// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
        /// <param name="proposedRate">A proposed person's hourly rate.</param>
        /// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
        public ComputedFinancialsEx CalculateProposedFinancials(int personId, decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount)
        {
            PersonRateCalculator calculator = new PersonRateCalculator(personId, true);
            return calculator.CalculateProposedFinancials(proposedRate, proposedHoursPerWeek, clientDiscount);
        }

        /// <summary>
        /// Calculates the person's rate.
        /// </summary>
        /// <param name="person">A <see cref="Person"/> object to calculate the data for.</param>
        /// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
        /// <param name="proposedRate">A proposed person's hourly rate.</param>
        /// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
        public ComputedFinancialsEx CalculateProposedFinancialsPerson(Person person, decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount, bool isMarginTestPage)
        {
            PersonRateCalculator calculator = GetCalculatorForProposedFinancials(person, proposedRate, proposedHoursPerWeek, isMarginTestPage);

            return calculator.CalculateProposedFinancials(proposedRate, proposedHoursPerWeek, clientDiscount);
        }

        /// <summary>
        /// Calculates the person's rate.
        /// </summary>
        /// <param name="person">A <see cref="Person"/> object to calculate the data for.</param>
        /// <param name="proposedHoursPerWeek">A proposed work week duration.</param>
        /// <param name="proposedRate">A proposed person's hourly rate.</param>
        /// <returns>The <see cref="ComputedRate"/> object with the calculation results.</returns>
        /// Not Using.
        public ComputedFinancialsEx CalculateProposedFinancialsPersonTargetMargin(Person person, decimal targetMargin, decimal proposedHoursPerWeek, decimal clientDiscount, bool isMarginTestPage)
        {
            PersonRateCalculator calculator = GetCalculatorForProposedFinancials(person, 0M, proposedHoursPerWeek, isMarginTestPage);
            return calculator.CalculateProposedFinancialsTargetMargin(targetMargin, proposedHoursPerWeek, clientDiscount);
        }

        private static PersonRateCalculator GetCalculatorForProposedFinancials(Person person, decimal proposedRate, decimal proposedHoursPerWeek, bool isMarginTestPage)
        {
            PersonRateCalculator calculator = new PersonRateCalculator(person, false, isMarginTestPage);
            if (person.CurrentPay != null)
            {
                person.OverheadList = PersonDAL.PersonOverheadListByTimescale(person.CurrentPay.Timescale);

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
                    person.CurrentPay.AmountHourly = person.CurrentPay.Amount / PersonRateCalculator.WorkingHoursInCurrentYear(proposedHoursPerWeek);

                //  Update hourly rate for percent of revenue persons with proposed rate
                if (person.CurrentPay.Timescale == TimescaleType.PercRevenue)
                {
                    person.CurrentPay.AmountHourly *= decimal.Multiply(proposedRate, 0.01M);
                }

                foreach (PersonOverhead overhead in person.OverheadList)
                {
                    if (overhead.RateType != null)
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
                                var defaultHoursPerYear = PersonRateCalculator.WorkingHoursInCurrentYear((int)proposedHoursPerWeek);
                                overhead.HourlyValue = overhead.Rate * 12 / defaultHoursPerYear;
                                break;
                            default:
                                overhead.HourlyValue = overhead.HourlyRate;
                                break;
                        }
                    }
                }
            }
            else
            {
                person.OverheadList = new List<PersonOverhead>();
            }

            person.OverheadList.Add(calculator.CalculateRecruitingOverhead());
            person.OverheadList.Add(calculator.CalculateBonusOverhead(proposedHoursPerWeek));
            return calculator;
        }

        /// <summary>
        /// Pertieves a payment for the specified person.
        /// </summary>
        /// <param name="personId">The <see cref="Person"/> to the data be retrieved for.</param>
        /// <param name="startDate">The StartDate since the payment is active.</param>
        /// <returns>The <see cref="Pay"/> object when found and null otherwise.</returns>
        public Pay GetPayment(int personId, DateTime startDate)
        {
            return PayDAL.GetByPersonStartDate(personId, startDate);
        }

        /// <summary>
        /// Saves a payment data.
        /// </summary>
        /// <param name="pay">The <see cref="Pay"/> object to be saved.</param>
        public void SavePay(Pay pay, string user = null)
        {
            PayDAL.SavePayDatail(pay, null, null, user);
        }

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
        public void SetNewPasswordForUser(string userName, string newPassword)
        {
            string salt = GenerateSalt();
            string hashedPassword = EncodePassword(newPassword, salt);
            PersonDAL.SetNewPasswordForUser(userName, hashedPassword, salt, 1, DateTime.UtcNow, "PracticeManagement");
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

        public int? SaveStrawman(Person person, Pay currentPay, string userLogin)
        {
            PersonDAL.SaveStrawMan(person, currentPay, userLogin);
            return person.Id;
        }

        public void DeleteStrawman(int personId, string userLogin)
        {
            PersonDAL.DeleteStrawman(personId, userLogin);
        }


        public int SaveStrawManFromExisting(int existingPersonId, Person person, string userLogin)
        {
            int newPersonId = 0;
            PersonDAL.SaveStrawManFromExisting(existingPersonId, person, out newPersonId, userLogin);
            return newPersonId;
        }


        public Person GetStrawmanDetailsById(int personId)
        {
            var person = PersonDAL.GetPersonFirstLastNameById(personId);
            person.PaymentHistory = PayDAL.GetHistoryByPerson(personId);
            return person;
        }

        public Person GetPersonDetailsShort(int personId)
        {
            return PersonDAL.GetPersonFirstLastNameById(personId);
        }


        public Person GetPayHistoryShortByPerson(int personId)
        {
            var person = new Person()
            {
                Id = personId
            };
            person.PaymentHistory = PayDAL.GetPayHistoryShortByPerson(personId);
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
        public List<Timescale> GetAllPayTypes()
        {
            return PersonDAL.GetAllPayTypes();
        }

        public Dictionary<DateTime, bool> IsPersonSalaryTypeListByPeriod(int personId, DateTime startDate, DateTime endDate)
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

        #endregion
    }
}

