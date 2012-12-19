﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;


namespace PracticeManagementService
{
    /// <summary>
    /// Provides the logic for the calculation of the person's rate.
    /// </summary>
    public class PersonRateCalculator
    {
        #region Constants

        private const string PersonArgument = "person";

        public const int DefaultHoursPerMonth = 160;
        public const int DefaultHoursPerWeek = 40;
        public const int DefaultHoursPerDay = 8;
        public const decimal MonthPerYear = 12.00M;
        public const decimal WeeksPerMonth = 4.20M;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or internally sets the person the rate be calculated for.
        /// </summary>
        public Person Person
        {
            get;
            private set;
        }

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new instance of the <see cref="PersonRateCalculator"/> class.
        /// </summary>
        /// <param name="person">The <see cref="Person"/>'s data the rate be calculated on.</param>
        public PersonRateCalculator(Person person)
            : this(person, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonRateCalculator"/> class.
        /// </summary>
        /// <param name="person">The <see cref="Person"/>'s data the rate be calculated on.</param>
        /// <param name="recalculateOverhead">Whether to recalculate overhead</param>
        /// <param name="loadAll">Load all comissions</param>
        public PersonRateCalculator(Person person, bool recalculateOverhead, bool isMarginTest = false, DateTime? effectiveDate = null)
        {
            Person = person;
            GetPersonDetail(Person, recalculateOverhead, isMarginTest, effectiveDate);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonRateCalculator"/> class.
        /// </summary>
        /// <param name="personId">An ID of the person the rate be calculated for.</param>
        public PersonRateCalculator(int personId)
            : this(personId, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonRateCalculator"/> class.
        /// </summary>
        /// <param name="personId">An ID of the person the rate be calculated for.</param>
        /// <param name="recalculateOverhead">Whether to recalculate overhead</param>
        public PersonRateCalculator(int personId, bool recalculateOverhead)
            : this(PersonDAL.GetById(personId), recalculateOverhead)
        {
        }

        #endregion

        #region Methods

        #region Data flow

        public void GetPersonDetail(Person person, bool recalculateOverhead, bool isMarginTest, DateTime? effectiveDate)
        {
            if (person != null && person.Id.HasValue && !isMarginTest)
            {
               
                if (effectiveDate == null)
                {
                    person.CurrentPay = PayDAL.GetCurrentByPerson(person.Id.Value);
                }

                person.OverheadList =
                    recalculateOverhead && person.CurrentPay != null ?
                    PersonDAL.PersonOverheadListByTimescale(person.CurrentPay.Timescale, effectiveDate) :
                    PersonDAL.PersonOverheadListByPerson(person.Id.Value, effectiveDate);

                foreach (PersonOverhead overhead in person.OverheadList)
                {
                    if (overhead.IsPercentage)
                    {
                        if (person.CurrentPay != null)
                        {
                            overhead.HourlyValue =
                                overhead.HourlyRate *
                                (person.CurrentPay.Timescale == TimescaleType.Salary ?
                                person.CurrentPay.Amount / WorkingHoursInCurrentYear() : person.CurrentPay.Amount) / 100M;
                        }
                        else
                        {
                            overhead.HourlyValue = 0;
                        }
                    }
                    else
                    {
                        overhead.HourlyValue = overhead.HourlyRate;
                    }
                }

                person.OverheadList.Add(CalculateVacationOverhead());
            }

            if (recalculateOverhead)
            {
                person.OverheadList.Add(CalculateBonusOverhead());
            }
        }

        /// <summary>
        /// Retrives the person details from the database.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <returns>The <see cref="Person"/> object.</returns>
        public static Person GetPersonDetail(int personId)
        {
            return new PersonRateCalculator(personId).Person;
        }

        /// <summary>
        /// Retrieves the list of persons who participate in the milestone.
        /// Calculates thier expense and profitability.
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <returns></returns>
        public static List<MilestonePerson> GetMilestonePersonListByMilestoneNoFinancials(int milestoneId)
        {
            return MilestonePersonDAL.MilestonePersonListByMilestone(milestoneId);
        }

        /// <summary>
        /// Retrieves the list of persons who participate in the milestone.
        /// Calculates thier expense and profitability.
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <returns></returns>
        public static List<MilestonePerson> GetMilestonePersonListByMilestone(int milestoneId)
        {
            var result = GetMilestonePersonListByMilestoneNoFinancials(milestoneId);
            ComputedFinancialsDAL.FinancialsGetByMilestonePersonsMonthly(milestoneId, result);
            ComputedFinancialsDAL.FinancialsGetByMilestonePersonsTotal(milestoneId, result);

            foreach (MilestonePerson milestonePerson in result)
            {
                ComputedFinancials mpComputedFinancials = milestonePerson.Entries[0].ComputedFinancials;
                PracticeManagementCurrency? billRate = null;

                if (mpComputedFinancials != null)
                    billRate = mpComputedFinancials.BillRate;

                if (billRate.HasValue)
                {
                    decimal discount = milestonePerson.Milestone.Project.Discount;
                    milestonePerson.Entries[0].ComputedFinancials.BillRateMinusDiscount = billRate - (billRate * (discount / 100));
                }

            }



            return result;
        }


        #endregion

        #region Financials

        #region What-if

        /// <summary>
        /// Calculates a monthly revenue
        /// </summary>
        /// <param name="hourlyRate"></param>
        /// <param name="hoursPerWeek"></param>
        /// <returns></returns>
        public static decimal CalculateMonthlyRevenue(decimal hourlyRate, decimal hoursPerWeek)
        {
            return Math.Round(hourlyRate * hoursPerWeek * WeeksPerMonth);
        }

        /// <summary>
        /// Calculates the financials for the <see cref="Person"/> with the proposed values.
        /// </summary>
        /// <param name="proposedRate">A proposed hourly bill rate.</param>
        /// <param name="proposedHoursPerWeek">A number of the billed hours per week.</param>
        /// <returns>A <see cref="ComputedFinancialsEx"/> object.</returns>
        public ComputedFinancialsEx CalculateProposedFinancials(decimal proposedRate, decimal proposedHoursPerWeek, decimal clientDiscount)
        {
            ComputedFinancialsEx financials = new ComputedFinancialsEx();

            foreach (PersonOverhead overhead in Person.OverheadList)
            {
                if (overhead.RateType != null &&
                    overhead.RateType.Id == (int)OverheadRateTypes.BillRateMultiplier)
                {
                    overhead.HourlyValue = proposedRate * overhead.BillRateMultiplier / 100M;
                }
            }

            financials.Revenue =
                PersonRateCalculator.CalculateMonthlyRevenue(proposedRate, proposedHoursPerWeek);

            financials.RevenueNet = financials.Revenue * (1 - clientDiscount);

            financials.OverheadList = Person.OverheadList;

            // Add the Vacation Overhead
            PersonOverhead vacationOverhead = CalculateVacationOverhead(proposedHoursPerWeek);
            financials.OverheadList.Add(vacationOverhead);

            financials.LoadedHourlyRate = Person.LoadedHourlyRate;

            // Add the Raw Hourly Rate
            //  For 1099/POR <Raw Hourly Rate> = [hourly bill rate]x[% of revenue]
            PersonOverhead rawHourlyRate = new PersonOverhead();
            rawHourlyRate.Name = Resources.Messages.RawHourlyRateTitle;
            rawHourlyRate.HoursToCollect = 1;
            rawHourlyRate.Rate = rawHourlyRate.HourlyValue =
                Person.CurrentPay.Timescale == TimescaleType.PercRevenue
                ?
                decimal.Multiply(proposedRate, (decimal)0.01) * Person.CurrentPay.Amount : Person.RawHourlyRate;


            decimal overheadSum = 0M;

            foreach (var overhead in financials.OverheadList.FindAll(OH => !OH.IsMLF))
            {
                overheadSum += overhead.HourlyValue;
            }

            financials.SemiLoadedHourlyRate = rawHourlyRate.HourlyValue + overheadSum;
            var RecruitingOverhead = financials.OverheadList.Find(OH => OH.Name == Resources.Messages.RecruitingOverheadName);

            financials.SemiLoadedHourlyRateWithoutRecruiting = financials.SemiLoadedHourlyRate - (RecruitingOverhead != null ? RecruitingOverhead.HourlyValue : 0);

            financials.SemiCOGS = financials.SemiLoadedHourlyRate.Value * proposedHoursPerWeek * WeeksPerMonth;
            financials.SemiCOGSWithoutRecruiting = financials.SemiLoadedHourlyRateWithoutRecruiting.Value * proposedHoursPerWeek * WeeksPerMonth;

            var MLFOverhead = financials.OverheadList.Find(OH => OH.IsMLF);
            if (MLFOverhead != null)
            {
                MLFOverhead.HourlyValue = MLFOverhead.Rate * rawHourlyRate.HourlyValue / 100;
                var FLHRWithoutMLF = financials.SemiLoadedHourlyRate;
                var FLHRWithMLF = rawHourlyRate.HourlyValue + MLFOverhead.HourlyValue;
                if (FLHRWithoutMLF > FLHRWithMLF)
                {
                    financials.LoadedHourlyRate = FLHRWithoutMLF;
                    MLFOverhead.HourlyValue = 0;
                }
                else
                {
                    financials.LoadedHourlyRate = FLHRWithMLF;
                    MLFOverhead.HourlyValue = FLHRWithMLF - FLHRWithoutMLF;
                }
            }
            else
            {
                financials.LoadedHourlyRate = financials.SemiLoadedHourlyRate;
            }
            var loadedHourlyRateWithoutRecruiting = financials.LoadedHourlyRate - (RecruitingOverhead != null ? RecruitingOverhead.HourlyValue : 0);


            financials.Cogs = financials.LoadedHourlyRate * proposedHoursPerWeek * WeeksPerMonth;
            financials.CogsWithoutRecruiting = loadedHourlyRateWithoutRecruiting * proposedHoursPerWeek * WeeksPerMonth;

            financials.GrossMargin = financials.RevenueNet - financials.Cogs;
            financials.MarginWithoutRecruiting = financials.RevenueNet - financials.CogsWithoutRecruiting;

            financials.OverheadList.Insert(0, rawHourlyRate);

            return financials;
        }

        #endregion

        #region Person rate calculations

        #region Rate for a Milestone


        /// <summary>
        /// Performs the calculation of person rate participating on the specific milestone.
        /// </summary>
        /// <param name="milestonePerson">The <see cref="MilestonePerson"/> association to the rate be calculated for.</param>
        /// <param name="userName">A current user.</param>
        /// <returns>The <see cref="PersonRate"/> object with the rate data.</returns>
        public static MilestonePerson CalculateRate(MilestonePerson milestonePerson, string userName)
        {
            MilestonePerson result;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                connection.Open();
                var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    // Saving data
                    MilestonePersonDAL.SaveMilestonePerson(milestonePerson, userName, connection, transaction);

                    // Retrieving the calculation result
                    result = milestonePerson;
                    result.ComputedFinancials = ComputedFinancialsDAL.FinancialsGetByMilestonePerson(
                                                        milestonePerson.Milestone.Id.Value,
                                                        milestonePerson.Person.Id.Value,
                                                        connection,
                                                        transaction);

                    result.Entries = MilestonePersonDAL.MilestonePersonEntryListByMilestonePersonId(milestonePerson.Id.Value, connection, transaction);

                    if (result.Milestone != null && result.Milestone.Id.HasValue &&
                        result.Person != null && result.Person.Id.HasValue)
                    {
                        // Financials for each entry
                        foreach (MilestonePersonEntry entry in result.Entries)
                        {
                            entry.ComputedFinancials =
                                ComputedFinancialsDAL.FinancialsGetByMilestonePersonEntry(entry.Id,connection,transaction);
                        }
                    }
                }
                finally
                {
                    // Rolling the transaction back
                    transaction.Rollback();
                }
            }

            return result;
        }

        #endregion

        #region COGS

        /// <summary>
        /// Computes the COGS for the person.
        /// </summary>
        /// <returns></returns>
        public PracticeManagementCurrency CalculateCogsForHours(
            decimal hours,
            decimal companyHours,
            decimal salesCommissionOverhead)
        {
            PracticeManagementCurrency monthAmount;
            PracticeManagementCurrency monthOverhead;

            if (Person == null || Person.CurrentPay == null)
            {
                // The pay was not specified yet
                monthAmount = new PracticeManagementCurrency();
                monthOverhead = new PracticeManagementCurrency();
            }
            else
            {
                // The basis pay
                switch (Person.CurrentPay.Timescale)
                {
                    case TimescaleType.Salary:
                        monthAmount = Person.CurrentPay.Amount / MonthPerYear;
                        monthOverhead =
                            Person != null ? (Person.TotalOverhead + salesCommissionOverhead) * companyHours : new PracticeManagementCurrency();
                        break;
                    case TimescaleType.Hourly:
                    case TimescaleType._1099Ctc:
                    case TimescaleType.PercRevenue:
                        monthAmount = Person.CurrentPay.Amount * hours;
                        monthOverhead =
                            Person != null ? (Person.TotalOverhead + salesCommissionOverhead) * hours : new PracticeManagementCurrency();
                        break;
                    default:
                        monthAmount = new PracticeManagementCurrency();
                        monthOverhead = new PracticeManagementCurrency();
                        break;
                }
            }

            // The real pay with overhead
            return monthAmount + monthOverhead;
        }

        public static int CompanyWorkDaysNumber(DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.WorkDaysCompanyNumber(startDate, endDate);
        }

        public decimal GetPersonWorkDays(DateTime startDate, DateTime endDate)
        {
            return GetPersonWorkDays(Person.Id.Value, startDate, endDate);
        }

        private decimal GetPersonWorkDays(int personId, DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.WorkDaysPersonNumber(personId, startDate, endDate) *
                   DefaultHoursPerDay;
        }

        /// <summary>
        /// Calculates a vacation overhead based on the <see cref="Person"/>'s expense.
        /// </summary>
        /// <returns>The <see cref="PersonOverhead"/> object with a calculated value.</returns>
        private PersonOverhead CalculateVacationOverhead(decimal hoursPerWeek = DefaultHoursPerWeek)
        {
            PersonOverhead result = new PersonOverhead();

            result.Name = Resources.Messages.VacationOverheadName;

            if (Person != null && Person.CurrentPay != null && Person.CurrentPay.VacationDays.HasValue)
            {
                // We have the data to culculate the vacation overhead.
                result.Rate =
                    Math.Round(
                    (Person.CurrentPay.VacationDays.Value * ((hoursPerWeek) / 5) * Person.RawHourlyRate) /
                    (decimal)WorkingHoursInCurrentYear(hoursPerWeek),
                    2);
                result.StartDate = Person.HireDate;
                result.HoursToCollect = 1;
                result.HourlyValue = result.Rate;
            }

            return result;
        }

        /// <summary>
        /// Calculates a bonus overhead based on the current <see cref="Pay"/>.
        /// </summary>
        /// <returns>The <see cref="PersonOverhead"/> object with a calculated value.</returns>
        public PersonOverhead CalculateBonusOverhead(decimal hoursPerWeek = DefaultHoursPerWeek)
        {
            PersonOverhead result = new PersonOverhead();
            result.Name = Resources.Messages.BonusOverheadName;
            if (Person.CurrentPay != null &&
                (Person.CurrentPay.BonusHoursToCollect.HasValue || Person.CurrentPay.IsYearBonus))
            {
                result.HoursToCollect =
                    Person.CurrentPay.IsYearBonus ?
                    (int)WorkingHoursInCurrentYear(hoursPerWeek) : Person.CurrentPay.BonusHoursToCollect.Value;
                result.Rate = Person.CurrentPay.BonusAmount;
                result.HourlyValue = result.HourlyRate;
            }
            return result;
        }

        public static decimal WorkingHoursInCurrentYear(decimal hoursPerWeek = 40)
        {

            //Working Hours per Year (52 * HPW))
            //var companyHolidays = CalendarDAL.GetCompanyHolidays(DateTime.Now.Year);
            var defaultHoursPerYear = (52 * hoursPerWeek);
            var yeartoCheckIfLeapYear = DateTime.Now.Year;
            if (yeartoCheckIfLeapYear > 0 && (((yeartoCheckIfLeapYear % 4) == 0) && ((yeartoCheckIfLeapYear % 100) != 0)
                    || ((yeartoCheckIfLeapYear % 400) == 0)))
            {
                defaultHoursPerYear = defaultHoursPerYear + hoursPerWeek / 5;
            }

            return defaultHoursPerYear;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        public static void VerifyPrivileges(string userName, ref string recruiterIds)
        {
            if (recruiterIds == null)
            {
                // Administrators can see anything.
                if (
                    !Roles.IsUserInRole(userName, Constants.RoleNames.AdministratorRoleName) &&
                    !Roles.IsUserInRole(userName, Constants.RoleNames.HRRoleName)                           //#2817:HRRoleName is added as per requirement.
                   )
                {
                    if (Roles.IsUserInRole(userName, Constants.RoleNames.RecruiterRoleName))
                    {
                        // A rectuiter can see only hes/her recruits
                        Person recruiter = PersonDAL.PersonGetByAlias(userName);
                        recruiterIds =
                            recruiter != null && recruiter.Id.HasValue ? recruiter.Id.Value.ToString() : null;
                    }
                    else
                    {
                        // Cannot apply the filter
                        recruiterIds = null;
                    }
                }
            }
        }
    }
}

