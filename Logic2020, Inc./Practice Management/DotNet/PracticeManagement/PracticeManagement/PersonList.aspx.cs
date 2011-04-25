using System;
using System.ComponentModel;
using System.Data;
using System.ServiceModel;
using System.Threading;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.Utils;

namespace PraticeManagement
{
    public partial class PersonList : PracticeManagementPageBase
    {
        private const string EditRecordCommand = "EditRecord";
        private const string DESCENDING = "DESC";
        private int? CurrentIndex
        {
            get
            {
                return Convert.ToInt32(Session["CurrentPageIndex"]);
            }
            set
            {
                Session["CurrentPageIndex"] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            odsPersons.TypeName = GetType().AssemblyQualifiedName;
            odsPersons.DataObjectTypeName = typeof(Person).AssemblyQualifiedName;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentIndex = 0;
            if (!IsPostBack)
            {
                bool userIsAdministrator =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                // Recruiters should see a complete list their recruits
                practiceFilter.ActiveOnly = userIsAdministrator;

                DataHelper.FillRecruiterList(
                    ddlRecruiter,
                    userIsAdministrator ? Resources.Controls.AnyRecruiterText : Resources.Controls.NotAvailableText,
                    null,
                    null);

                if (!userIsAdministrator)
                {
                    Person current = DataHelper.CurrentPerson;

                    ddlRecruiter.SelectedIndex =
                        ddlRecruiter.Items.IndexOf(
                            ddlRecruiter.Items.FindByValue(
                                current != null && current.Id.HasValue ? current.Id.Value.ToString() : string.Empty));
                    ddlRecruiter.Enabled = false;

                    btnExportToExcel.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        /// <summary>
        /// Retrives the data and display them in the table.
        /// </summary>
        protected override void Display()
        {
            gvPersons.DataBind();
        }

        /// <summary>
        /// Retrieves the number of records for the ObjectDataSource.
        /// </summary>
        /// <param name="practiceId">The filter by the Practice.</param>
        /// <param name="active">The filter by the Activity flag</param>
        /// <param name="pageSize">The size of the data page.</param>
        /// <param name="pageNo">The number of the data page.</param>
        /// <param name="looked">The text from search text box.</param>
        /// <param name="recruiterId">The recruiter filter.</param>
        /// <returns>The total number of records to be paged.</returns>
        public static int GetPersonCount(int? practiceId, bool active, int pageSize, int pageNo, string looked,
                                         string recruiterId)
        {
            //if (CurrentIndex != -1 && pageNo == 0)
            //{
            //    pageNo = CurrentIndex;
            //}
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    int? recruiter =
                        !string.IsNullOrEmpty(recruiterId) ? (int?)int.Parse(recruiterId) : null;
                    return
                        serviceClient.GetPersonCount(
                            practiceId,
                            active,
                            looked,
                            recruiter,
                            Thread.CurrentPrincipal.Identity.Name);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrives the data for the ObjectDataSource.
        /// </summary>
        /// <param name="practiceId">The filter by the Practice.</param>
        /// <param name="active">The filter by the Activity flag</param>
        /// <param name="pageSize">The size of the data page.</param>
        /// <param name="pageNo">The number of the data page.</param>
        /// <param name="looked">The text from search text box.</param>
        /// <param name="startRow">Actually does not used.</param>
        /// <param name="maxRows">Actually does not used.</param>
        /// <param name="recruiterId">The recruiter filter.</param>
        /// <param name="sortBy"></param>
        /// <returns>The list of the <see cref="Person"/> object for the specified data page.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static Person[] GetPersons(int? practiceId, bool active, int pageSize, int pageNo, string looked,
                                          int startRow, int maxRows, string recruiterId, string sortBy)
        {
            //if (CurrentIndex != -1 && pageNo == 0)
            //{
            //    pageNo = CurrentIndex;
            //}
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    int? recruiter =
                        !string.IsNullOrEmpty(recruiterId) ? (int?)int.Parse(recruiterId) : null;
                    Person[] result =
                        serviceClient.GetPersonListWithCurrentPay(
                            practiceId,
                            active,
                            pageSize,
                            pageNo,
                            looked,
                            recruiter,
                            Thread.CurrentPrincipal.Identity.Name,
                            sortBy);

                    //Array.Sort(result, (x, y) => SortFunction(sortBy, x, y));

                    return result;
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private static int SortFunction(string sortBy, Person x, Person y)
        {
            IComparable cx, cy;

            bool desc = sortBy.Contains(DESCENDING);
            if (desc)
                sortBy = sortBy.Replace(DESCENDING, string.Empty).TrimEnd();

            switch (sortBy)
            {
                case "HireDate":
                    cx = x.HireDate;
                    cy = y.HireDate;
                    break;

                case "TerminationDate":
                    DateTime? xtd = x.TerminationDate;
                    DateTime? ytd = y.TerminationDate;

                    if (xtd.HasValue && ytd.HasValue)
                    {
                        cx = xtd.Value;
                        cy = ytd.Value;
                    }
                    else
                    {
                        cx = DateTime.MinValue;
                        cy = DateTime.MinValue;
                    }
                    break;

                case "Practice":
                    cx = x.DefaultPractice == null ? string.Empty : x.DefaultPractice.Name;
                    cy = y.DefaultPractice == null ? string.Empty : y.DefaultPractice.Name;
                    break;

                case "TimescaleName":
                    cx = x.CurrentPay == null ? string.Empty : x.CurrentPay.TimescaleName;
                    cy = y.CurrentPay == null ? string.Empty : y.CurrentPay.TimescaleName;
                    break;

                case "Status":
                    cx = x.Status.Name;
                    cy = y.Status.Name;
                    break;

                case "RawHourlyRate":
                    desc = !desc;
                    bool payExists = x.CurrentPay != null && y.CurrentPay != null;

                    if (payExists)
                    {
                        bool xIsPor = x.CurrentPay.Timescale == TimescaleType.PercRevenue;
                        bool yIsPor = y.CurrentPay.Timescale == TimescaleType.PercRevenue;
                        if ((xIsPor && !yIsPor) || (!xIsPor && yIsPor))
                        {
                            cx = x.CurrentPay.Timescale;
                            cy = y.CurrentPay.Timescale;

                            return CompResult(cx, cy, desc);
                        }
                    }

                    cx = x.RawHourlyRate.Value;
                    cy = y.RawHourlyRate.Value;
                    break;

                case "Seniority":
                    //  Assign lowest seniority to the ones who have no one
                    cx = x.Seniority == null ? 105 : x.Seniority.Id;
                    cy = y.Seniority == null ? 105 : y.Seniority.Id;
                    break;

                case "Manager":
                    //  Assign lowest seniority to the ones who have no one
                    cx = x.Manager.PersonLastFirstName;
                    cy = y.Manager.PersonLastFirstName;
                    break;

                default:
                    cx = x.LastName;
                    cy = y.LastName;
                    break;
            }

            return CompResult(cx, cy, desc);
        }

        private static int CompResult(IComparable cx, IComparable cy, bool desc)
        {
            int result = cx.CompareTo(cy);
            return desc ? -result : result;
        }

        protected void gvPersons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == EditRecordCommand)
            {
                object args = e.CommandArgument;
                Redirect(GetPersonDetailsUrl(args));
            }
        }

        private static string GetPersonDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.PersonDetail,
                                 args);
        }

        protected string GetPersonDetailsUrlWithReturn(object id)
        {
            return Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(id), Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// Refreshes the data in the table after the filter was changed
        /// </summary>
        /// <param name="sebder"></param>
        /// <param name="e"></param>
        protected void practiceFilter_FilterChanged(object sebder, EventArgs e)
        {
            Display();
        }


        /// <summary>
        /// Searches the persons by first name or last name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            practiceFilter.ActiveOnly = false;
            Display();

            if (gvPersons.Rows.Count == 1)
            {
                Person person = ((Person[])odsPersons.Select())[0];
                Response.Redirect(
                    Urls.GetPersonDetailsUrl(person, Request.Url.AbsoluteUri));
            }
        }

        protected void ddlRecruiter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    DataHelper.InsertExportActivityLogMessage("Person");

                    DataSet excelData =
                        serviceClient.PersonGetExcelSet();
                    excelGrid.DataSource = excelData;
                    excelGrid.DataMember = "excelDataTable";
                    excelGrid.DataBind();
                    excelGrid.Visible = true;
                    GridViewExportUtil.Export("Person_List.xls", excelGrid);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void gvPersons_Sort(object sender, GridViewSortEventArgs e)
        {
            CurrentIndex = this.gvPersons.PageIndex;
        }

        protected void gvPersons_DataBound(object sender, EventArgs e)
        {
            this.gvPersons.PageIndex = (!CurrentIndex.HasValue ? 0 : CurrentIndex.Value);
        }
        protected void gvPersons_PageIndexChanged(object sender, EventArgs e)
        {
            CurrentIndex = this.gvPersons.PageIndex;
        }

        public string FormatDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

