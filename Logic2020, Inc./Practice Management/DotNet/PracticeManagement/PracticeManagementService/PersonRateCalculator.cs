using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Security;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;
using System.Data;
using System.Linq;


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

        private const int MaxIterationNumber = 10000;
        private const decimal RateCalculationSteps = 10M;
        private const decimal RateCalculationThreshold = 0.01M;

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
        public PersonRateCalculator(Person person, bool recalculateOverhead, bool isMarginTest = false)
        {
            Person = person;
            GetPersonDetail(Person, recalculateOverhead, isMarginTest);
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

        public void GetPersonDetail(Person person, bool recalculateOverhead, bool isMarginTest)
        {
            if (person != null && person.Id.HasValue && !isMarginTest)
            {
                person.DefaultPersonRecruiterCommission =
                    DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionListByPerson(person.Id.Value);
                person.CurrentPay = PayDAL.GetCurrentByPerson(person.Id.Value);
                person.RecruiterCommission =
                        RecruiterCommissionDAL.DefaultRecruiterCommissionListByRecruitId(person.Id.Value);

                person.DefaultPersonCommissions = DefaultCommissionDAL.DefaultCommissionListByPerson(person.Id.Value);

                person.OverheadList =
                    recalculateOverhead && person.CurrentPay != null ?
                    PersonDAL.PersonOverheadListByTimescale(person.CurrentPay.Timescale) :
                    PersonDAL.PersonOverheadListByPerson(person.Id.Value);

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
                person.OverheadList.Add(CalculateRecruitingOverhead());
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

        /// <summary>
        /// Checks whether the specified user is an Administrator and leaves or replaces the value to filter data.
        /// </summary>
        /// <param name="userName">The user to be verified.</param>
        /// <param name="recruiterId">An ID of the recruiter for filter.</param>
        public static void VerifyPrivileges(string userName, ref int? recruiterId)
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
                    recruiterId =
                        recruiter != null && recruiter.Id.HasValue ? recruiter.Id.Value : 0;
                }
                else
                {
                    // Cannot apply the filter
                    recruiterId = 0;
                }
            }
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
        /// Calculates a projected margin.
        /// </summary>
        /// <param name="hourlyRate">The projected horly rate.</param>
        /// <param name="hoursPerWeek">The projected numder of hours per week.</param>
        /// <returns>The projected margin for the company.</returns>
        public decimal CalculateProjectedMargin(decimal hourlyRate, decimal hoursPerWeek, decimal salesCommission)
        {
            return CalculateMonthlyRevenue(hourlyRate, hoursPerWeek) - CalculateCogs(hoursPerWeek, salesCommission);
        }

        /// <summary>
        /// Calculates a default sales commission for the proposed rate.
        /// </summary>
        /// <param name="hourlyRate">The projected horly rate.</param>
        /// <param name="hoursPerWeek">The projected numder of hours per week.</param>
        /// <returns>The projected sales commission.</returns>
        public decimal CalculateSalesCommission(decimal hourlyRate, decimal hoursPerWeek, decimal defaultSalesCommission)
        {
            // Task: #756
            // The calculation should be exactly:
            //SalesCommission = Round((BillRate – RawHourlyRate) *.05)
            decimal tmpResult =
                hourlyRate - (Person.CurrentPay != null ? Person.CurrentPay.AmountHourly.Value : 0M);

            //return tmpResult * Constants.Finacials.DefaultSalesCommission;
            return tmpResult * defaultSalesCommission;

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
            financials.OverheadList.Insert(2, vacationOverhead);

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
            financials.MarginWithoutRecruiting = financials.Revenue - financials.CogsWithoutRecruiting;

            //if (MLFOverhead != null && MLFOverhead.HourlyValue > 0)
            //{
            //    financials.SalesCommission = financials.GrossMargin * defaultSalesCommission;
            //    financials.SaleCommissionPerHour = financials.SalesCommission / (proposedHoursPerWeek * WeeksPerMonth);
            //}

            // Add a sales commission overhead
            //PersonOverhead salesCommission = new PersonOverhead();
            //salesCommission.Name = Resources.Messages.SalesCommissionTitle;
            //salesCommission.HoursToCollect = 1;
            //salesCommission.Rate = salesCommission.HourlyValue = financials.SaleCommissionPerHour;
            //Person.OverheadList.Add(salesCommission);

            financials.OverheadList.Insert(0, rawHourlyRate);

            return financials;
        }

        /// <summary>
        /// Calculates the financials for the <see cref="Person"/> with the proposed values.
        /// </summary>
        /// <param name="targetMargin">The Target Margin in percentage.</param>
        /// <param name="proposedHoursPerWeek">A number of the billed hours per week.</param>
        /// <returns>A <see cref="ComputedFinancialsEx"/> object.</returns>
        public ComputedFinancialsEx CalculateProposedFinancialsTargetMargin(decimal targetMargin, decimal proposedHoursPerWeek, decimal clientDiscount)
        {
            // Determine the first approximation
            decimal proposedRate = CalculateCogs(proposedHoursPerWeek, 0M);
            proposedRate =
                proposedRate / (WeeksPerMonth * proposedHoursPerWeek) +
                (proposedRate * targetMargin) / (100M * WeeksPerMonth * proposedHoursPerWeek);
            proposedRate =
                //CalculateCogs(proposedHoursPerWeek, proposedRate * Constants.Finacials.DefaultSalesCommission);
                CalculateCogs(proposedHoursPerWeek, 0M);
            proposedRate =
                proposedRate / (WeeksPerMonth * proposedHoursPerWeek) +
                (proposedRate * targetMargin) / (100M * WeeksPerMonth * proposedHoursPerWeek);

            ComputedFinancialsEx financials = CalculateProposedFinancials(proposedRate, proposedHoursPerWeek, clientDiscount);

            int iterationNumber = MaxIterationNumber;
            while (iterationNumber > 0 && (Math.Abs(targetMargin - financials.TargetMargin) > RateCalculationThreshold))
            {
                // Removing recalculated overheads.
                var vacationOverhead = Person.OverheadList.Find(oh => oh.Name == Resources.Messages.VacationOverheadName);
                var rawHourlyRate = Person.OverheadList.Find(oh => oh.Name == Resources.Messages.RawHourlyRateTitle);
                //var salesCommission = Person.OverheadList.Find(oh => oh.Name == Resources.Messages.SalesCommissionTitle);
                if (vacationOverhead != null)
                {
                    Person.OverheadList.Remove(vacationOverhead);  // remove the Vacation Overhead
                }

                if (rawHourlyRate != null)
                {
                    Person.OverheadList.Remove(rawHourlyRate); // remove the Raw Hourly Rate
                }
                //if (salesCommission != null)
                //{
                //    Person.OverheadList.Remove(salesCommission); // remove a sales commission overhead
                //}

                proposedRate +=
                    financials.Cogs * (targetMargin - financials.TargetMargin) /
                    (100M * WeeksPerMonth * proposedHoursPerWeek * RateCalculationSteps);
                financials = CalculateProposedFinancials(proposedRate, proposedHoursPerWeek, clientDiscount);

                iterationNumber--;
            }

            financials.HoursBilled = WeeksPerMonth * proposedHoursPerWeek;

            return financials;
        }

        #endregion

        #region Person rate calculations

        #region Rate for a Milestone

        /// <summary>
        /// Calculates the financial counters for each person in the list and for each
        /// month within the specified period.
        /// </summary>
        [Obsolete]
        internal static Dictionary<DateTime, ComputedFinancials> CalculateFinancials(DateTime periodStart,
            DateTime periodEnd,
            List<Milestone> milestones,
            List<MilestonePerson> projectParticipants)
        {
            Dictionary<DateTime, ComputedFinancials> result = new Dictionary<DateTime, ComputedFinancials>();

            // Calculate the project financials by months
            for (DateTime dtTemp = periodStart;
                dtTemp <= periodEnd;
                dtTemp = dtTemp.AddMonths(1))
            {
                ComputedFinancials interestValue = null;

                if (milestones != null)
                {
                    // Determines whether some milestone occurs in this month
                    foreach (Milestone milestone in milestones)
                    {
                        DateTime dtMilestoneStartMonth =
                            new DateTime(milestone.StartDate.Year, milestone.StartDate.Month, 1);
                        DateTime dtMilestoneEndMonth =
                            new DateTime(milestone.ProjectedDeliveryDate.Year,
                                milestone.ProjectedDeliveryDate.Month,
                                DateTime.DaysInMonth(milestone.ProjectedDeliveryDate.Year, milestone.ProjectedDeliveryDate.Month));

                        if (dtTemp >= dtMilestoneStartMonth && dtTemp <= dtMilestoneEndMonth)
                        {
                            // Financials for milestones without persons
                            if (milestone.PersonCount == 0 && !milestone.IsHourlyAmount)
                            {
                                // Initailize the result buffer
                                if (interestValue == null)
                                {
                                    interestValue = new ComputedFinancials();
                                }

                                DateTime monthStart = new DateTime(dtTemp.Year, dtTemp.Month, 1);
                                DateTime monthEnd =
                                    new DateTime(dtTemp.Year, dtTemp.Month, DateTime.DaysInMonth(dtTemp.Year, dtTemp.Month));

                                int milestoneMonthDays =
                                    CalendarDAL.WorkDaysCompanyNumber(
                                    monthStart < milestone.StartDate ? milestone.StartDate : monthStart,
                                    monthEnd > milestone.ProjectedDeliveryDate ? milestone.ProjectedDeliveryDate : monthEnd);

                                decimal revenue =
                                    milestone.ProjectedDuration != 0 && milestone.Amount.HasValue ?
                                    milestone.Amount.Value.Value * milestoneMonthDays / milestone.ProjectedDuration : 0M;
                                decimal revenueNet =
                                    milestone.ProjectedDuration != 0 && milestone.Project != null ?
                                    revenue - (revenue * milestone.Project.Discount / 100M) : revenue;

                                interestValue.Revenue += revenue;
                                interestValue.RevenueNet += revenueNet;
                                interestValue.GrossMargin += revenueNet;
                            }
                        }
                    }
                }

                foreach (MilestonePerson milestonePerson in projectParticipants)
                {
                    decimal revenue = 0M;
                    decimal cogs = 0M;

                    foreach (MilestonePersonEntry entry in milestonePerson.Entries)
                    {
                        foreach (BilledTime billedTime in entry.EstimatedWorkloadByMonth)
                        {
                            if (billedTime.DateBilled.Year == dtTemp.Year &&
                                billedTime.DateBilled.Month == dtTemp.Month)
                            {
                                if (interestValue == null)
                                {
                                    // Initailize the result buffer
                                    interestValue = new ComputedFinancials();
                                }

                                decimal revenueTmp =
                                    entry.HourlyAmount.HasValue ?
                                    entry.HourlyAmount.Value.Value * billedTime.HoursBilled : 0M;
                                decimal revenueNet =
                                    revenueTmp - (revenueTmp * milestonePerson.Milestone.Project.Discount / 100M);

                                interestValue.Revenue += revenueTmp;
                                interestValue.RevenueNet += revenueNet;
                                interestValue.Cogs += entry.ComputedFinancials.Cogs;
                                interestValue.GrossMargin += entry.ComputedFinancials.GrossMargin;
                                interestValue.HoursBilled += billedTime.HoursBilled;

                                revenue += revenueTmp;
                                cogs += entry.ComputedFinancials.Cogs;
                            }
                        }
                    }

                    if (milestones != null)
                    {
                        // Revenue for milestones
                        foreach (Milestone milestone in milestones)
                        {
                            if (milestone.Id == milestonePerson.Milestone.Id &&
                                milestone.IsHourlyAmount &&
                                interestValue != null)
                            {
                                milestone.Amount += revenue;
                            }
                        }
                    }
                }

                if (interestValue != null)
                {
                    // Include only the necessary results
                    result.Add(dtTemp, interestValue);
                }
            }

            return result;
        }

        private static void CalculateRateByEntry(MilestonePerson milestonePerson)
        {
            foreach (MilestonePersonEntry entry in milestonePerson.Entries)
            {
                entry.ComputedFinancials =
                    ComputedFinancialsDAL.FinancialsGetByMilestonePersonEntry(
                    milestonePerson.Milestone.Id.Value,
                    milestonePerson.Person.Id.Value,
                    entry.StartDate);
            }
        }

        /// <summary>
        /// Calculates an expected expense of each <see cref="Person"/> assigned to the <see cref="Milestone"/>.
        /// </summary>
        /// <param name="persons">The list of the person-milstone pairs.</param>
        /// <returns>The total expense.</returns>
        [Obsolete]
        public static void CalculateExpense(IEnumerable<MilestonePerson> persons)
        {
            foreach (MilestonePerson milestonePerson in persons)
            {
                foreach (MilestonePersonEntry entry in milestonePerson.Entries)
                {
                    if (milestonePerson.Person != null && milestonePerson.Person.Id.HasValue)
                    {
                        CalculateRateByEntry(milestonePerson);
                    }

                    entry.EstimatedClientDiscount =
                        entry.ComputedFinancials != null ?
                            (entry.ComputedFinancials.Revenue *
                            (milestonePerson.Milestone != null && milestonePerson.Milestone.Project != null ?
                            milestonePerson.Milestone.Project.Discount : 0M) / 100) : new PracticeManagementCurrency();
                }
            }
        }

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
                                ComputedFinancialsDAL.FinancialsGetByMilestonePersonEntry(
                                result.Milestone.Id.Value,
                                result.Person.Id.Value,
                                entry.StartDate,
                                connection,
                                transaction);
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
        public PracticeManagementCurrency CalculateCogs(decimal hoursPerWeek, decimal salesCommissionOverhead)
        {
            return CalculateCogsForHours(
                hoursPerWeek * WeeksPerMonth,
                DefaultHoursPerWeek * WeeksPerMonth,
                salesCommissionOverhead);
        }

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

        /// <summary>
        /// Computes the COGS for the person for an especial period.
        /// </summary>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <returns></returns>
        public PracticeManagementCurrency CalculateCogs(DateTime startDate, DateTime endDate)
        {
            decimal personHours = GetPersonWorkDays(startDate, endDate);
            decimal companyHours = CompanyWorkDaysNumber(startDate, endDate) * DefaultHours;

            return CalculateCogsForHours(personHours, companyHours, 0M);
        }

        public static int CompanyWorkDaysNumber(DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.WorkDaysCompanyNumber(startDate, endDate);
        }

        public decimal DefaultHours
        {
            get { return (Person.CurrentPay != null ? Person.CurrentPay.DefaultHoursPerDay : 0M); }
        }

        public decimal GetPersonWorkDays(DateTime startDate, DateTime endDate)
        {
            return GetPersonWorkDays(Person.Id.Value, startDate, endDate);
        }

        private decimal GetPersonWorkDays(int personId, DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.WorkDaysPersonNumber(personId, startDate, endDate) *
                   DefaultHours;
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
        /// Calculates a recruiting overhead.
        /// </summary>
        /// <returns>The <see cref="PersonOverhead"/> object with a calculated value.</returns>
        public PersonOverhead CalculateRecruitingOverhead()
        {
            PersonOverhead result = new PersonOverhead();
            result.Name = Resources.Messages.RecruitingOverheadName;
            result.HoursToCollect = 1;

            if (Person.RecruiterCommission != null && Person.CurrentPay != null)
            {
                int hoursWorked;

                if (Person.Id.HasValue && Person.HireDate == DateTime.MinValue)
                {
                    var tempPerson = PersonDAL.GetById(Person.Id.Value);
                    if (tempPerson != null)
                    {
                        Person.HireDate = tempPerson.HireDate;
                    }
                }
                if (Person.HireDate == DateTime.MinValue)
                {

                    // Calculation for a new person
                    hoursWorked = 0;
                }
                else
                {
                    hoursWorked =
                        (int)Math.Floor(
                        Math.Max((DateTime.Today - Person.HireDate).TotalDays, 0.0) *
                        (double)DefaultHoursPerDay);
                }

                foreach (RecruiterCommission commision in Person.RecruiterCommission)
                {
                    if (commision.HoursToCollect >= hoursWorked && commision.HoursToCollect != 0)
                    {
                        result.Rate += commision.Amount / commision.HoursToCollect;
                    }
                }

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
    }
}

