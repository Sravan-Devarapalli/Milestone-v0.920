using System;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using DataTransferObjects;
using PraticeManagement.ExpenseService;
using PraticeManagement.ProjectService;
using PraticeManagement.Controls.Reports;

namespace PraticeManagement.Controls
{
	/// <summary>
	/// Provides the state storage for the filter on the Company Performance page.
	/// </summary>
	[Serializable]
	public class CompanyPerformanceState
	{
		#region Constants

		private const string CompanyPerformanceFilterKey = "CurrentCompanyPerformanceFilterSet";
		private const string CompanyPerformanceDataKey = "CurrentCompanyPerformanceDataSet";

		#endregion

		#region Properties

		#region Instance props

		/// <summary>
		/// Gets or sets a list of projects to be displayed.
		/// </summary>
		public Project[] ProjectListState
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of benches to be displayed.
		/// </summary>
		public Project[] BenchListState
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an expenses list to be displayed.
		/// </summary>
		public MonthlyExpense[] ExpenseListState
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the data for the person stats report.
		/// </summary>
		public PersonStats[] PersonStatsState
		{
			get;
			set;
		}

		#endregion

		/// <summary>
		/// Gets a current user session
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// If the session state is unavailable at this point in the HTTP pipeline.
		/// </exception>
		private static HttpSessionState Session
		{
			get
			{
				HttpSessionState result;
				if (HttpContext.Current == null)
				{
					result = null;
				}
				else if (HttpContext.Current.Session == null)
				{
					throw new InvalidOperationException(Resources.Messages.SessionStateUnavailable);
				}
				else
				{
					result = HttpContext.Current.Session;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets or sets a Company Performance data for the specified filter.
		/// </summary>
		private static CompanyPerformanceState State
		{
			get
			{
				CompanyPerformanceState result;

				if (Session == null)
				{
					result = null;
				}
				else
				{
					result = Session[CompanyPerformanceDataKey] as CompanyPerformanceState;

					if (result == null)
					{
						result = new CompanyPerformanceState();
						Session[CompanyPerformanceDataKey] = result;
					}
				}

				return result;
			}
			set
			{
				if (Session != null)
				{
					Session[CompanyPerformanceDataKey] = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the filter applied to the data in the current storage.
		/// </summary>
		public static CompanyPerformanceFilterSettings Filter
		{
			get
			{
				CompanyPerformanceFilterSettings result;

				if (Session == null)
				{
					result = null;
				}
				else
				{
					result = Session[CompanyPerformanceFilterKey] as CompanyPerformanceFilterSettings;

					if (result == null)
					{
						result = new CompanyPerformanceFilterSettings();
						Session[CompanyPerformanceFilterKey] = result;
					}
				}

				return result;
			}
			set
			{
				if (Session != null)
				{
					if (Filter != value)
					{
						Clear();
					}

					Session[CompanyPerformanceFilterKey] = value;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets a list of projects to be displayed.
		/// </summary>
		/// <remarks>From or to singleton.</remarks>
		public static Project[] ProjectList
		{
			get
			{
                CompanyPerformanceState singleton = State;
				CompanyPerformanceFilterSettings filterSet = Filter;
				Project[] result;

				if (singleton != null && filterSet != null)
				{
					result = singleton.ProjectListState;

					if (result == null)
					{
						using (var serviceClient = new ProjectServiceClient())
						{
							try
							{
                                if (!Filter.IsGroupByPersonPage)
                                {
                                    result =
                                        serviceClient.ProjectListAllMultiParameters(
                                        filterSet.ClientIdsList,
                                        filterSet.ShowProjected,
                                        filterSet.ShowCompleted,
                                        filterSet.ShowActive,
                                        filterSet.ShowInternal,
                                        filterSet.ShowExperimental,
                                        filterSet.ShowInactive,
                                        filterSet.PeriodStart,
                                        filterSet.PeriodEnd,
                                        Thread.CurrentPrincipal.Identity.Name,
                                        filterSet.SalespersonIdsList,
                                        filterSet.ProjectOwnerIdsList,
                                        filterSet.PracticeIdsList,
                                        filterSet.ProjectGroupIdsList,
                                        !filterSet.TotalOnlySelectedDateWindow,
                                        filterSet.ExcludeInternalPractices);
                                }
                                else
                                {
                                    result =
                                        serviceClient.GetProjectListWithFinancials(
                                        filterSet.ClientIdsList,
                                        filterSet.ShowProjected,
                                        filterSet.ShowCompleted,
                                        filterSet.ShowActive,
                                        filterSet.ShowInternal,
                                        filterSet.ShowExperimental,
                                        filterSet.ShowInactive,
                                        filterSet.PeriodStart,
                                        filterSet.PeriodEnd,
                                        filterSet.SalespersonIdsList,
                                        filterSet.ProjectOwnerIdsList,
                                        filterSet.PracticeIdsList,
                                        filterSet.ProjectGroupIdsList,
                                        filterSet.ExcludeInternalPractices);
                                }
							}
							catch (CommunicationException ex)
							{
								serviceClient.Abort();
								throw;
							}
						}

						singleton.ProjectListState = result;
						State = singleton;
					}
				}
				else
				{
					result = null;
				}

				return result;
			}
			set
			{
				var singleton = State;
				if (State != null)
				{
					State.ProjectListState = value;
					State = singleton;
				}
			}
		}

		/// <summary>
		/// Gets or sets a list of benches to be displayed.
		/// </summary>
		/// <remarks>From or to singleton.</remarks>
		public static Project[] BenchList
		{
			get
			{
				CompanyPerformanceState singleton = State;
				CompanyPerformanceFilterSettings filterSet = Filter;
				Project[] result;

				if (singleton != null && filterSet != null)
				{
                    result = 
                        singleton.BenchListState 
                        ?? 
                        ReportsHelper.GetBenchList(filterSet.PeriodStart, filterSet.PeriodEnd, DataHelper.CurrentPerson.Alias);

					singleton.BenchListState = result;
					State = singleton;
				}
				else
				{
					result = null;
				}

				return result;
			}
			set
			{
				CompanyPerformanceState singleton = State;
				if (State != null)
				{
					State.BenchListState = value;
					State = singleton;
				}
			}
		}

		/// <summary>
		/// Gets or sets an expenses list to be displayed.
		/// </summary>
		public static MonthlyExpense[] ExpenseList
		{
			get
			{
				var singleton = State;
				var filterSet = Filter;
				MonthlyExpense[] result;

				if (singleton != null && filterSet != null)
				{
					result = singleton.ExpenseListState;

					if (result == null)
					{
						using (var serviceClient = new ExpenseServiceClient())
						{
							try
							{
								result = serviceClient.MonthlyExpenseListAll(filterSet.PeriodStart, filterSet.PeriodEnd);
							}
							catch (CommunicationException)
							{
								serviceClient.Abort();
								throw;
							}
						}

						singleton.ExpenseListState = result;
						State = singleton;
					}
				}
				else
				{
					result = null;
				}

				return result;
			}
			set
			{
				CompanyPerformanceState singleton = State;
				if (State != null)
				{
					State.ExpenseListState = value;
					State = singleton;
				}
			}
		}

		/// <summary>
		/// Gets or sets the data for the person stats report.
		/// </summary>
		public static PersonStats[] PersonStats
		{
			get
			{
				CompanyPerformanceState singleton = State;
				CompanyPerformanceFilterSettings filterSet = Filter;
				PersonStats[] result;

				if (singleton != null && filterSet != null)
				{
					result = singleton.PersonStatsState;

					if (result == null)
					{
						using (var serviceClient = new ProjectServiceClient())
						{
							try
							{
                                result = GetPersonStats(filterSet.PeriodStart,
                                                            filterSet.PeriodEnd,
                                                            Thread.CurrentPrincipal.Identity.Name,
                                                            filterSet.SalespersonId,
                                                            filterSet.ProjectOwnerId,
                                                            false,
                                                            true,
                                                            true,
                                                            true,
                                                            true,
                                                            true);
							}
							catch (CommunicationException)
							{
								serviceClient.Abort();
								throw;
							}
						}

						singleton.PersonStatsState = result;
						State = singleton;
					}
				}
				else
				{
					result = null;
				}

				return result;
			}
			set
			{
				CompanyPerformanceState singleton = State;
				if (State != null)
				{
					State.PersonStatsState = value;
					State = singleton;
				}
			}
		}


        public static PersonStats[] GetPersonStats(DateTime periodStart, DateTime periodEnd, string identityName, int? salesPersonId,
                                                          int? projectOwnerId, bool showProjected, bool showCompleted, bool showActive, bool showExperimental,bool showInternal, bool showInactive)
        {
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    return serviceClient.PersonStartsReport(periodStart, periodEnd, identityName, salesPersonId,
                                                            projectOwnerId, showProjected, showCompleted, showActive, showExperimental, showInternal, showInactive);

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

		#endregion

		#region Methods

		/// <summary>
		/// Removes the data from the session state.
		/// </summary>
		public static void Clear()
		{
			if (Session != null)
			{
				Session[CompanyPerformanceDataKey] = null;
			}
		}

		#endregion
	}
}

