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
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Config
{
    public partial class Persons : PracticeManagementPageBase
    {
        #region Fields

        private const string AllRecruiters = "All Recruiters";
        private const string EditRecordCommand = "EditRecord";
        private const string DESCENDING = "DESC";
        private const string ViewStateSortColumnId = "SortColumnId";
        private const string ViewStateSortExpression = "SortExpression";
        private const string ViewStateSortDirection = "SortDirection";
        private const string ViewingRecords = "Viewing {0} of {1} Records";

        #endregion

        #region Properties

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

        private char? Alphabet
        {
            get
            {
                char result;

                if (char.TryParse(hdnAlphabet.Value, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }

            }
        }

        private string previousLetter
        {
            get
            {
                string value;

                value = Session["PreviousLetter"] != null ? (string)Session["PreviousLetter"] : null;

                return value;
            }
            set
            {
                Session["PreviousLetter"] = value;
            }
        }

        private string GridViewSortColumnId
        {
            get
            {
                return (string)ViewState[ViewStateSortColumnId];
            }
            set
            {
                ViewState[ViewStateSortColumnId] = value;
            }
        }

        private string PrevGridViewSortExpression
        {
            get
            {
                return (string)ViewState[ViewStateSortExpression];
            }
            set
            {
                ViewState[ViewStateSortExpression] = value;
            }
        }

        private string GridViewSortDirection
        {
            get
            {
                return (string)ViewState[ViewStateSortDirection];
            }
            set
            {
                ViewState[ViewStateSortDirection] = value;
            }
        }

        private string PracticeIdsSelected
        {
            get
            {
                return hdnPracticeId.Value;
            }
        }

        private string PayTypeIdsSelected
        {
            get
            {
                return hdnPayTypeId.Value;
            }
        }

        #endregion Properties

        #region Events And Methods

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            Display();

            if (!IsPostBack)
            {
                CurrentIndex = 0;

                bool userIsAdministrator =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                bool userIsHR =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName); //#2817: userIsHR is added as per  requirement.

                // Recruiters should see a complete list their recruits
                //practiceFilter.ActiveOnly = userIsAdministrator || userIsHR; //#2817: userIsHR is added as per  requirement.


                personsFilter.Active = true;   //Always active on load
                //personsFilter.Projected = personsFilter.Terminated = personsFilter.Inactive = !(userIsAdministrator || userIsHR);

                DataHelper.FillRecruiterList(
                    cblRecruiters,
                     AllRecruiters,
                    null,
                    null);//#2817: userIsHR is added as per  requirement.

                if (!userIsAdministrator && !userIsHR)//#2817: userIsHR is added as per  requirement.
                {
                    Person current = DataHelper.CurrentPerson;

                    var selectedItem = cblRecruiters.Items.FindByValue(current != null && current.Id.HasValue ? current.Id.Value.ToString() : string.Empty);
                    if (selectedItem != null)
                    {
                    }

                    for (int i = cblRecruiters.Items.Count - 1; i >= 1; i--)
                    {
                        if (!(cblRecruiters.Items[i].Value == current.Id.Value.ToString()))
                        {
                            cblRecruiters.Items.RemoveAt(i);
                        }
                    }

                    btnExportToExcel.Visible = false;
                }

                SelectAllItems(this.cblRecruiters);
                previousLetter = lnkbtnAll.ID;

                gvPersons.Sort("LastName", SortDirection.Ascending);
                SetFilterValues();
            }

            AddAlphabetButtons();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        #endregion

        #region ControlEvents

        protected void gvPersons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == EditRecordCommand)
            {
                object args = e.CommandArgument;
                Response.Redirect(GetPersonDetailsUrl(args));
            }
        }

        /// <summary>
        /// Refreshes the data in the table after the filter was changed
        /// </summary>
        /// <param name="sebder"></param>
        /// <param name="e"></param>
        protected void personsFilter_FilterChanged(object sebder, EventArgs e)
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
            personsFilter.Active = true;
            personsFilter.Projected = true;
            personsFilter.Terminated = true;
            personsFilter.Inactive = true;
            Display();

            if (gvPersons.Rows.Count == 1)
            {
                Person person = ((Person[])odsPersons.Select())[0];
                Response.Redirect(
                    Urls.GetPersonDetailsUrl(person, Request.Url.AbsoluteUri));
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            hdnLooked.Value = txtSearch.Text;
            gvPersons.PageSize = GetPageSize(ddlView.SelectedValue);
        }

        protected void Alphabet_Clicked(object sender, EventArgs e)
        {
            CurrentIndex = 0;
            if (previousLetter != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLetter);
                previousLinkButton.Font.Bold = false;
            }

            LinkButton alpha = (LinkButton)sender;
            alpha.Font.Bold = true;
            hdnAlphabet.Value = alpha.Text != "All" ? alpha.Text : null;

            previousLetter = alpha.ID;
            gvPersons.PageSize = GetPageSize(ddlView.SelectedValue);
        }

        protected void UpdateView_Clicked(object sender, EventArgs e)
        {
            CurrentIndex = 0;
            SetFilterValues();
            gvPersons.PageSize = GetPageSize(ddlView.SelectedValue);
        }

        protected void ResetFilter_Clicked(object sender, EventArgs e)
        {
            ResetFilterControlsToDefault();
            SetFilterValues();
            gvPersons.PageSize = GetPageSize(ddlView.SelectedValue);
            gvPersons.Sort("LastName", SortDirection.Ascending);
            gvPersons.PageIndex = 0;
            CurrentIndex = 0;
        }

        protected void DdlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPersons.PageSize = GetPageSize(((DropDownList)sender).SelectedValue);
        }

        protected void Previous_Clicked(object sender, EventArgs e)
        {
            if (CurrentIndex != null && CurrentIndex > 0)
            {
                CurrentIndex = (int)CurrentIndex - 1;
            }
        }

        protected void Next_Clicked(object sender, EventArgs e)
        {
            if (CurrentIndex != null && gvPersons.Rows.Count != 0)
            {
                CurrentIndex = (int)CurrentIndex + 1;
            }
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

        protected void gvPersons_Sorting(object sender, GridViewSortEventArgs e)
        {
            CurrentIndex = this.gvPersons.PageIndex;


            if (PrevGridViewSortExpression != e.SortExpression)
            {
                PrevGridViewSortExpression = e.SortExpression;
                GridViewSortDirection = e.SortDirection.ToString();
            }
            else
            {
                GridViewSortDirection = GetSortDirection();
            }

            //GridViewSortColumnId = GetSortColumnId(e.SortExpression);
        }

        protected void gvPersons_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var row = e.Row;

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    TableCell cell = row.Cells[i];

                    if (cell.HasControls())
                    {
                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is LinkButton)
                            {
                                var lb = (LinkButton)ctrl;

                                if (lb.CommandArgument == PrevGridViewSortExpression)
                                {
                                    lb.CssClass = "arrow";
                                    lb.CssClass += string.Format(" sort-{0}", GridViewSortDirection == "Ascending" ? "up" : "down");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void gvPersons_DataBound(object sender, EventArgs e)
        {
            this.gvPersons.PageIndex = (!CurrentIndex.HasValue ? 0 : CurrentIndex.Value);
        }

        protected void gvPersons_PageIndexChanged(object sender, EventArgs e)
        {
            CurrentIndex = this.gvPersons.PageIndex;
        }

        protected void gvPersons_PreRender(object sender, EventArgs e)
        {
            int currentRecords = gvPersons.Rows.Count;
            int totalRecords = GetTotalRecords("-1");
            lblRecords.Text = String.Format(ViewingRecords, currentRecords, totalRecords);

            if (ddlView.SelectedValue == "-1")
            {
                lnkbtnPrevious.Enabled = lnkbtnNext.Enabled = false;
            }
            else
            {
                lnkbtnPrevious.Enabled = !(CurrentIndex == 0);
                lnkbtnNext.Enabled = !((gvPersons.Rows.Count == 0) || (currentRecords == totalRecords) || (currentRecords < Convert.ToInt32(ddlView.SelectedValue)));
            }
        }

        #endregion

        #region StaticMethods

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
        public static int GetPersonCount(string practiceIdsSelected, bool active, int pageSize, int pageNo, string looked,
                                         string recruitersSelected, string payTypeIdsSelected, bool projected, bool terminated, bool inactive, char? alphabet)
        {
            if (practiceIdsSelected == null)
            {
                practiceIdsSelected = string.Empty;
            }

            if (payTypeIdsSelected == null)
            {
                payTypeIdsSelected = string.Empty;
            }

            if (recruitersSelected == null)
            {
                recruitersSelected = string.Empty;
            }

            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return
                        serviceClient.GetPersonCountByCommaSeperatedIdsList(
                            practiceIdsSelected,
                            active,
                            looked,
                            recruitersSelected,
                            Thread.CurrentPrincipal.Identity.Name,
                            payTypeIdsSelected,
                            projected,
                            terminated,
                            inactive,
                            alphabet);
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
        public static Person[] GetPersons(string practiceIdsSelected, bool active, int pageSize, int pageNo, string looked,
                                          int startRow, int maxRows, string recruitersSelected, string sortBy, string payTypeIdsSelected,
                                            bool projected, bool terminated, bool inactive, char? alphabet)
        {
            if (practiceIdsSelected == null)
            {
                practiceIdsSelected = string.Empty;
            }

            if (payTypeIdsSelected == null)
            {
                payTypeIdsSelected = string.Empty;
            }

            if (recruitersSelected == null)
            {
                recruitersSelected = string.Empty;
            }

            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] result =
                        serviceClient.GetPersonListWithCurrentPayByCommaSeparatedIdsList(
                            practiceIdsSelected,
                            active,
                            pageSize,
                            pageNo,
                            looked,
                            recruitersSelected,
                            Thread.CurrentPrincipal.Identity.Name,
                            sortBy,
                            payTypeIdsSelected,
                            projected,
                            terminated,
                            inactive,
                            alphabet);

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

        private static string GetPersonDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.PersonDetail,
                                 args);
        }

        #endregion

        #region Methods

        protected override void Display()
        {
        }

        private void AddAlphabetButtons()
        {
            for (int index = 65; index <= 65 + 25; index++)
            {
                char alphabet = Convert.ToChar(index);

                LinkButton Alphabet = new LinkButton();
                Alphabet.ID = "lnkbtn" + alphabet;

                HtmlTableCell tc = new HtmlTableCell();
                tc.ID = "td" + alphabet;
                tc.Style.Add("padding-left", "15px");
                tc.Style.Add("padding-top", "10px");
                tc.Style.Add("padding-bottom", "10px");
                tc.Style.Add("text-align", "center");

                Alphabet.Text = alphabet.ToString();
                Alphabet.Font.Underline = false;
                Alphabet.Click += new EventHandler(Alphabet_Clicked);

                tc.Controls.Add(Alphabet);

                trAlphabeticalPaging.Controls.Add(tc);
            }
        }

        /// <summary>
        /// Retrives the data and display them in the table.
        /// </summary>
        protected void DisplayContent()
        {
            gvPersons.DataBind();
        }

        protected string GetPersonDetailsUrlWithReturn(object id)
        {
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(id), Request.Url.AbsoluteUri);
        }

        private string GetSortDirection()
        {
            switch (GridViewSortDirection)
            {
                case "Ascending":
                    GridViewSortDirection = "Descending";
                    break;
                case "Descending":
                    GridViewSortDirection = "Ascending";
                    break;
            }
            return GridViewSortDirection;
        }

        private void SetFilterValues()
        {
            hdnActive.Value = personsFilter.Active.ToString();
            hdnPracticeId.Value = personsFilter.PracticeIds;
            hdnRecruiterId.Value = cblRecruiters.SelectedItems;
            hdnPayTypeId.Value = personsFilter.PayTypeIds;
            hdnProjected.Value = personsFilter.Projected.ToString();
            hdnTerminated.Value = personsFilter.Terminated.ToString();
            hdnInactive.Value = personsFilter.Inactive.ToString();
            hdnLooked.Value = txtSearch.Text;
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        private void ResetFilterControlsToDefault()
        {
            CheckBox activeOnly = (CheckBox)personsFilter.FindControl("chbShowActive");
            CheckBox projected = (CheckBox)personsFilter.FindControl("chbProjected");
            CheckBox terminated = (CheckBox)personsFilter.FindControl("chbTerminated");
            CheckBox inactive = (CheckBox)personsFilter.FindControl("chbInactive");

            personsFilter.ResetFilterControlsToDefault();
            SelectAllItems(this.cblRecruiters);

            activeOnly.Checked = true;
            projected.Checked = terminated.Checked = inactive.Checked = false;
            txtSearch.Text = string.Empty;
            ddlView.SelectedIndex = 0;

            //Reset to All button.
            if (previousLetter != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLetter);
                previousLinkButton.Font.Bold = false;
            }

            lnkbtnAll.Font.Bold = true;
            previousLetter = lnkbtnAll.ID;
            hdnAlphabet.Value = null;
        }

        private int GetPageSize(string view)
        {
            int pageSize = GetTotalRecords(view);

            return pageSize != 0 ? pageSize : 1;
        }

        private int GetTotalRecords(string view)
        {
            int pageSize = Convert.ToInt32(view);

            if (pageSize == -1)
            {
                pageSize = GetPersonCount(PracticeIdsSelected,
                                            Convert.ToBoolean(hdnActive.Value),
                                            0,
                                            1,
                                            hdnLooked.Value,
                                            hdnRecruiterId.Value,
                                            PayTypeIdsSelected,
                                            Convert.ToBoolean(hdnProjected.Value),
                                            Convert.ToBoolean(hdnTerminated.Value),
                                            Convert.ToBoolean(hdnInactive.Value),
                                            Alphabet);
            }

            return pageSize;
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

        #endregion

        #endregion
    }
}

