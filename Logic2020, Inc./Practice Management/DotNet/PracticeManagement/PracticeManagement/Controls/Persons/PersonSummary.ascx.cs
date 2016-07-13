using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using PraticeManagement.Utils.Excel;
using PraticeManagement.Utils;
using System.ServiceModel;
using DataTransferObjects.Filters;
using DataTransferObjects;
using System.Web.Security;
using System.Threading;
using PraticeManagement.PersonService;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using PraticeManagement.FilterObjects;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonSummary : System.Web.UI.UserControl
    {
        #region Fields

        private const string AllRecruiters = "All Recruiters";
        private const string EditRecordCommand = "EditRecord";
        private const string DESCENDING = "DESC";
        private const string ViewStateSortColumnId = "SortColumnId";
        private const string ViewStateSortExpression = "SortExpression";
        private const string ViewStateSortDirection = "SortDirection";
        private const string ViewingRecords = "Viewing {0} - {1} of {2} Persons";
        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";
        private const int NameCharactersLength = 50;
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;
        private bool? _userIsAdministratorValue;
        private bool? _userIsOperationsValue;
        private bool? _userIsHRValue;
        private bool? _userIsRecruiterValue;

        #endregion

        #region Properties

        private PraticeManagement.Config.PersonReport HostingPageIsPersonReport
        {
            get
            {
                if (Page is PraticeManagement.Config.PersonReport)
                {
                    return ((PraticeManagement.Config.PersonReport)Page);
                }
                else
                {
                    return null;
                }
            }
        }

        private PraticeManagement.Config.Persons HostingPageIsPersons
        {
            get
            {
                if (Page is PraticeManagement.Config.Persons)
                {
                    return ((PraticeManagement.Config.Persons)Page);
                }
                else
                {
                    return null;
                }
            }
        }

        private PraticeManagement.DashBoard PreviousPage
        {
            get
            {
                if (HostingPageIsPersonReport != null)
                {
                    return HostingPageIsPersonReport.PreviousPage;
                }
                else if (HostingPageIsPersons != null)
                {
                    return HostingPageIsPersons.PreviousPage;
                }
                else
                {
                    return null;
                }
            }
        }

        public string RecruiterIds
        {
            get
            {
                if (cblRecruiters.Items[0].Selected)
                {
                    return null;
                }
                return cblRecruiters.SelectedItems;
            }
            set
            {
                cblRecruiters.SelectedItems = value;
            }
        }

        public string ExMessage { get; set; }

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

        private string RecruiterIdsSelectedKey
        {
            get
            {
                return Session["RecruiterIdsSelectedKey"] as string;
            }
            set
            {
                Session["RecruiterIdsSelectedKey"] = value;
            }
        }

        private string PayTypeIdsSelectedKey
        {
            get
            {
                return Session["PayTypeIdsSelectedKey"] as string;
            }
            set
            {
                Session["PayTypeIdsSelectedKey"] = value;
            }
        }

        private string PracticeIdsSelectedKey
        {
            get
            {
                return Session["PracticeIdsSelectedKey"] as string;
            }
            set
            {
                Session["PracticeIdsSelectedKey"] = value;
            }
        }

        private string DivisionIdsSelectedKey
        {
            get
            {
                return Session["DivisionIdsSelectedKey"] as string;
            }
            set
            {
                Session["DivisionIdsSelectedKey"] = value;
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
            set
            {
                if (value != null)
                {
                    hdnAlphabet.Value = value.ToString();
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

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles wrapdataCellStyle = new CellStyles();
                wrapdataCellStyle.WrapText = true;

                CellStyles dataDateCellStyle = new CellStyles();
                dataDateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles dataNumberDataCellStyle = new CellStyles();
                dataNumberDataCellStyle.DataFormat = "$#,##0.00";

                CellStyles dataNumberDateCellStyle1 = new CellStyles();
                dataNumberDateCellStyle1.DataFormat = "#,##0";

                CellStyles[] dataCellStylearray;
                if (HostingPageIsPersons != null)
                {
                    dataCellStylearray = new CellStyles[] { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataCellStyle,
                                                    dataDateCellStyle,
                                                    dataDateCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataNumberDataCellStyle,
                                                    dataNumberDataCellStyle,
                                                    dataNumberDataCellStyle,
                                                    dataNumberDateCellStyle1,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle
                                                  };


                }
                else
                {
                    dataCellStylearray = new CellStyles[] { dataCellStyle,
                                                    dataCellStyle, 
                                                    dataCellStyle, 
                                                    dataDateCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataCellStyle,
                                                    dataNumberDataCellStyle,
                                                    dataCellStyle,
                                                  };
                }

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

        protected bool UserIsAdministrator
        {
            get
            {
                if (!_userIsAdministratorValue.HasValue)
                {
                    _userIsAdministratorValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }

                return _userIsAdministratorValue.Value;
            }
        }

        protected bool UserIsOperations
        {
            get
            {
                if (!_userIsOperationsValue.HasValue)
                {
                    _userIsOperationsValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.OperationsRoleName);
                }

                return _userIsOperationsValue.Value;
            }
        }

        protected bool UserIsHR
        {
            get
            {
                if (!_userIsHRValue.HasValue)
                {
                    _userIsHRValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
                }

                return _userIsHRValue.Value;
            }
        }

        protected bool UserIsRecruiter
        {
            get
            {
                if (!_userIsRecruiterValue.HasValue)
                {
                    _userIsRecruiterValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
                }

                return _userIsRecruiterValue.Value;
            }
        }

        #endregion Properties

        #region Events And Methods

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            //HostingPageIsPersonReport.Display();
            AddAlphabetButtons();
            if (!IsPostBack)
            {
                bool userIsAdministrator =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                bool userIsHR =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName); //#2817: userIsHR is added as per  requirement.

                // Recruiters should see a complete list their recruits
                //practiceFilter.ActiveOnly = userIsAdministrator || userIsHR; //#2817: userIsHR is added as per  requirement.

                DataHelper.FillRecruiterList(cblRecruiters, AllRecruiters, true);//#2817: userIsHR is added as per  requirement.

                if (!userIsAdministrator && !userIsHR && !UserIsOperations)//#2817: userIsHR is added as per  requirement.
                {
                    Person current = DataHelper.CurrentPerson;

                    for (int i = cblRecruiters.Items.Count - 1; i >= 1; i--)
                    {
                        if (!(cblRecruiters.Items[i].Value == current.Id.Value.ToString()))
                        {
                            cblRecruiters.Items.RemoveAt(i);
                        }
                    }
                }
                btnExportToExcel.Visible = userIsAdministrator || userIsHR;
                var cookie = SerializationHelper.DeserializeCookie(Constants.FilterKeys.PersonFilterCookie) as PersonFilter;

                if (Request.QueryString[Constants.FilterKeys.ApplyFilterFromCookieKey] == "true" && cookie != null)
                {
                    CurrentIndex = cookie.CurrentIndex;
                    personsFilter.Active = cookie.ShowActive;
                    personsFilter.TerminationPending = cookie.ShowTerminationPending;
                    personsFilter.Projected = cookie.ShowProjected;
                    personsFilter.Terminated = cookie.ShowTerminated;
                    personsFilter.PracticeIds = cookie.SelectedPracticeIds;
                    personsFilter.PayTypeIds = cookie.SelectedPayTypeIds;
                    cblRecruiters.SelectedItems = cookie.SelectedRecruiterIds;
                    ddlView.SelectedIndex = cookie.SelectedPageSizeIndex;
                    Alphabet = cookie.Alphabet;
                    txtSearch.Text = cookie.SearchText;
                    if (PreviousPage != null)
                    {
                        txtSearch.Text = PreviousPage.SearchText;
                    }

                    gvPersons.Sort(cookie.SortBy, cookie.SortOrder);
                    SetFilterValues();

                    if (cookie.Alphabet.HasValue)
                    {
                        LinkButton topButton = (LinkButton)trAlphabeticalPaging.FindControl("lnkbtn" + cookie.Alphabet.Value);
                        //LinkButton bottomButton = (LinkButton)trAlphabeticalPaging1.FindControl("lnkbtn1" + cookie.Alphabet.Value);
                        topButton.CssClass = "fontBold";
                        lnkbtnAll.CssClass = "fontNormal";
                        hdnAlphabet.Value = topButton.Text != "All" ? topButton.Text : null;
                        previousLetter = topButton.ID;
                    }
                    else if (cookie.DisplaySearchResuts)
                    {
                        SearchPersons();
                        SaveFilterSettings();
                    }
                }
                else
                {
                    if (PreviousPage != null)
                    {
                        txtSearch.Text = PreviousPage.SearchText;
                        personsFilter.Projected = true;
                        personsFilter.TerminationPending = true;
                        personsFilter.Terminated = true;
                    }


                    CurrentIndex = 0;
                    personsFilter.Active = true;   //Always active on load
                    SelectAllItems(this.cblRecruiters);
                    previousLetter = lnkbtnAll.ID;
                    gvPersons.Sort("LastName", SortDirection.Ascending);
                    SetFilterValues();
                }

                GetFilterValuesForSession();

            }
            if (HostingPageIsPersonReport != null)
            {
                lnkAddPerson.Visible = false;
            }

            gvPersons.EmptyDataText = "No results found.";

            ScriptManager.RegisterStartupScript(this, GetType(), "", "changeAlternateitemscolrsForCBL();", true);
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
            //Display();
        }

        protected void btnSearchAll_OnClick(object sender, EventArgs e)
        {
            SearchPersons();
            SaveFilterSettings();

            SaveFilterValuesForSession();

        }

        private void SearchPersons()
        {
            CurrentIndex = 0;
            bool istrue = true;
            hdnActive.Value = istrue.ToString();
            hdnProjected.Value = istrue.ToString();
            hdnTerminated.Value = istrue.ToString();
            hdnTerminatedPending.Value = istrue.ToString();
            PracticeIdsSelectedKey = null;
            RecruiterIdsSelectedKey = null;
            PayTypeIdsSelectedKey = null;
            hdnLooked.Value = txtSearch.Text;
            hdnAlphabet.Value = null;
            gvPersons.DataBind();
            hdnCleartoDefaultView.Value = "true";
            btnClearResults.Enabled = true;
            if (gvPersons.Rows.Count == 0)
            {
                string txt = txtSearch.Text;
                txt = "<b>" + txt + "</b>";
                gvPersons.EmptyDataText = string.Format("No results found for {0}", txt);
            }

            if (previousLetter != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLetter);
                LinkButton prevtopButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLinkButton.Attributes["Top"]);
                //LinkButton prevbottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(previousLinkButton.Attributes["Bottom"]);
                prevtopButton.CssClass = "fontNormal";
                //prevbottomButton.Font.Bold = false;
            }

        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchPersons();
        }

        protected void Alphabet_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            CurrentIndex = 0;
            if (previousLetter != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLetter);
                LinkButton prevtopButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLinkButton.Attributes["Top"]);
                //LinkButton prevbottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(previousLinkButton.Attributes["Bottom"]);

                prevtopButton.CssClass = "fontNormal";
                // prevbottomButton.Font.Bold = false;
            }

            LinkButton alpha = (LinkButton)sender;

            LinkButton topButton = (LinkButton)trAlphabeticalPaging.FindControl(alpha.Attributes["Top"]);
            //LinkButton bottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(alpha.Attributes["Bottom"]);

            topButton.CssClass = "fontBold";
            //bottomButton.Font.Bold = true;
            hdnAlphabet.Value = topButton.Text != "All" ? topButton.Text : null;
            previousLetter = topButton.ID;

            if (hdnCleartoDefaultView.Value == "true")
            {
                ResetControls();
                SetFilterValues();
                hdnCleartoDefaultView.Value = "false";
            }
            SaveFilterSettings();
            gvPersons.DataBind();
        }

        protected void UpdateView_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            hdnCleartoDefaultView.Value = "false";
            txtSearch.Text = string.Empty;
            CurrentIndex = 0;
            SetFilterValues();
            SaveFilterSettings();

            SaveFilterValuesForSession();

            gvPersons.DataBind();
        }

        private void SaveFilterSettings()
        {
            if (HostingPageIsPersons != null)
            {
                PersonFilter filter = GetFilterSettings();
                SerializationHelper.SerializeCookie(filter, Constants.FilterKeys.PersonFilterCookie);
            }

        }

        private PersonFilter GetFilterSettings()
        {
            var filter = new PersonFilter
            {
                SearchText = txtSearch.Text,
                SelectedPageSizeIndex = ddlView.SelectedIndex,
                SelectedPayTypeIds = personsFilter.PayTypeIds,
                SelectedPracticeIds = personsFilter.PracticeIds,
                SelectedRecruiterIds = cblRecruiters.Items[0].Selected ? null : cblRecruiters.SelectedItems,
                ShowActive = personsFilter.Active,
                ShowTerminationPending = personsFilter.TerminationPending,
                ShowProjected = personsFilter.Projected,
                ShowTerminated = personsFilter.Terminated,
                Alphabet = Alphabet,
                CurrentIndex = CurrentIndex,
                SortOrder = GridViewSortDirection == "Ascending" ? SortDirection.Ascending : SortDirection.Descending,
                SortBy = PrevGridViewSortExpression,
                DisplaySearchResuts = hdnCleartoDefaultView.Value == "true"

            };
            return filter;
        }

        protected void ResetFilter_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            hdnCleartoDefaultView.Value = "false";
            ResetFilterControlsToDefault();
            SetFilterValues();
            gvPersons.Sort("LastName", SortDirection.Ascending);
            gvPersons.PageIndex = 0;
            CurrentIndex = 0;
            SaveFilterSettings();

            SaveFilterValuesForSession();

        }

        protected void DdlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPersons.PageIndex = 0;
            CurrentIndex = 0;
            gvPersons.DataBind();
            SaveFilterSettings();

            SaveFilterValuesForSession();

        }

        protected void Previous_Clicked(object sender, EventArgs e)
        {
            if (CurrentIndex != null && CurrentIndex > 0)
            {
                CurrentIndex = (int)CurrentIndex - 1;
            }
            SaveFilterSettings();
            gvPersons.DataBind();
        }

        protected void Next_Clicked(object sender, EventArgs e)
        {
            if (CurrentIndex != null && gvPersons.Rows.Count != 0)
            {
                CurrentIndex = (int)CurrentIndex + 1;
            }
            SaveFilterSettings();
            gvPersons.DataBind();
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    if (HostingPageIsPersons != null)
                    {
                        DataHelper.InsertExportActivityLogMessage("Person");
                        var dataSetList = new List<DataSet>();
                        List<SheetStyles> sheetStylesList = new List<SheetStyles>();
                        DataSet excelData =
                            serviceClient.PersonGetExcelSet();
                        headerRowsCount = 1;
                        coloumnsCount = excelData.Tables[0].Columns.Count;
                        sheetStylesList.Add(DataSheetStyle);
                        excelData.DataSetName = "Person_List";
                        dataSetList.Add(excelData);
                        NPOIExcel.Export("Person_List.xls", dataSetList, sheetStylesList);
                    }
                    else if (HostingPageIsPersonReport != null)
                    {
                        DataHelper.InsertExportActivityLogMessage("Person Summary Report");
                        var dataSetList = new List<DataSet>();
                        List<SheetStyles> sheetStylesList = new List<SheetStyles>();
                        DataSet excelData = serviceClient.PersonGetExcelSetWithFilters(personsFilter.PracticeIds, personsFilter.DivisionIds, hdnLooked.Value, RecruiterIds, personsFilter.PayTypeIds, personsFilter.Active, personsFilter.Projected, personsFilter.Terminated, personsFilter.TerminationPending);
                        sheetStylesList.Add(DataSheetStyle);
                        excelData.DataSetName = "PersonSummaryReport";
                        dataSetList.Add(excelData);
                        NPOIExcel.Export("PersonSummaryReport.xls", dataSetList, sheetStylesList);
                    }
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
            SaveFilterSettings();
            //GridViewSortColumnId = GetSortColumnId(e.SortExpression);
        }

        protected void gvPersons_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var row = e.Row;
                LinkButton lnk = row.FindControl("lnkHireDate") as LinkButton;
                lnk.Text =HostingPageIsPersonReport != null?"Hire Date":"Start Date";
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
            if (HostingPageIsPersonReport != null)
            {
                gvPersons.Columns[3].Visible = false;
                gvPersons.Columns[7].Visible = false;
                gvPersons.Columns[10].Visible = false;

             

            }
            if (HostingPageIsPersons != null)
            {
                gvPersons.Columns[8].Visible = false;
            }
        }

        protected void gvPersons_PageIndexChanged(object sender, EventArgs e)
        {
            CurrentIndex = this.gvPersons.PageIndex;
        }

        protected void gvPersons_PreRender(object sender, EventArgs e)
        {
            int currentRecords = gvPersons.Rows.Count;
            int totalRecords = GetTotalRecords("-1");
            int startIndex = currentRecords == 0 ? 0 : (gvPersons.PageIndex == 0 ? 1 : (gvPersons.PageIndex * Convert.ToInt32(ddlView.SelectedValue)) + 1);
            lblRecords.Text = String.Format(ViewingRecords, startIndex, currentRecords == 0 ? 0 : (startIndex + currentRecords - 1), totalRecords);

            if (ddlView.SelectedValue == "-1")
            {
                lnkbtnPrevious.Enabled = lnkbtnNext.Enabled = false;
            }
            else
            {
                lnkbtnPrevious.Enabled = !(CurrentIndex == 0);
                lnkbtnNext.Enabled = !((gvPersons.Rows.Count == 0) || (currentRecords == totalRecords) || (currentRecords < Convert.ToInt32(ddlView.SelectedValue)));
            }

            if (!lnkbtnPrevious.Enabled)
            {
                Color color = ColorTranslator.FromHtml("#8F8F8F");
                lnkbtnPrevious.ForeColor = color;
                //lnkbtnPrevious1.ForeColor = color;
            }
            else
            {
                Color color = ColorTranslator.FromHtml("#0898E6");
                lnkbtnPrevious.ForeColor = color;
                // lnkbtnPrevious1.ForeColor = color;
            }
            if (!lnkbtnNext.Enabled)
            {
                Color color = ColorTranslator.FromHtml("#8F8F8F");
                lnkbtnNext.ForeColor = color;
                // lnkbtnNext1.ForeColor = color;
            }
            else
            {
                Color color = ColorTranslator.FromHtml("#0898E6");
                lnkbtnNext.ForeColor = color;
                // lnkbtnNext1.ForeColor = color;
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
        public static int GetPersonCount(string practiceIdsSelected, string divisionIdsSelected, bool active, int pageSize, int pageNo, string looked,
                                         string recruitersSelected, string payTypeIdsSelected, bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    return
                        serviceClient.GetPersonCountByCommaSeperatedIdsList(
                            practiceIdsSelected,
                            divisionIdsSelected,
                            active,
                            looked,
                            recruitersSelected,
                            Thread.CurrentPrincipal.Identity.Name,
                            payTypeIdsSelected,
                            projected,
                            terminated,
                            terminatedPending,
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
        public static Person[] GetPersons(string practiceIdsSelected, string divisionIdsSelected, bool active, int pageSize, int pageNo, string looked,
                                          int startRow, int maxRows, string recruitersSelected, string sortBy, string payTypeIdsSelected,
                                            bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person[] result =
                        serviceClient.GetPersonListWithCurrentPayByCommaSeparatedIdsList(
                            practiceIdsSelected,
                            divisionIdsSelected,
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
                            terminatedPending,
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
                    cx = x.Seniority == null ? 105 : Seniority.GetSeniorityValueById(x.Seniority.Id);
                    cy = y.Seniority == null ? 105 : Seniority.GetSeniorityValueById(y.Seniority.Id);
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

        //protected override void Display()
        //{

        //}

        private void AddAlphabetButtons()
        {
            for (int index = 65; index <= 65 + 25; index++)
            {
                char alphabet = Convert.ToChar(index);

                LinkButton Alphabet = new LinkButton();
                Alphabet.ID = "lnkbtn" + alphabet;
                Alphabet.Attributes.Add("Top", "lnkbtn" + alphabet);
                Alphabet.Attributes.Add("Bottom", "lnkbtn1" + alphabet);

                HtmlTableCell tc = new HtmlTableCell();
                tc.ID = "td" + alphabet;
                tc.Attributes["class"] = "TcPersons";


                Alphabet.Text = alphabet.ToString();
                Alphabet.CssClass = "NoUnderline";
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

        protected string GetPersonDetailsUrlWithReturn(object obj)
        {
            int personId;
            if (obj is Person)
            {
                var person = obj as Person;
                personId = person.Id.Value;
            }
            else if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                personId = (int)obj;
            }
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(personId),
                       Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));
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
            PracticeIdsSelectedKey = personsFilter.PracticeIds;
            RecruiterIdsSelectedKey = RecruiterIds;
            PayTypeIdsSelectedKey = personsFilter.PayTypeIds;
            DivisionIdsSelectedKey = personsFilter.DivisionIds;
            hdnProjected.Value = personsFilter.Projected.ToString();
            hdnTerminated.Value = personsFilter.Terminated.ToString();
            hdnTerminatedPending.Value = personsFilter.TerminationPending.ToString();
            hdnLooked.Value = txtSearch.Text;
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        private void ResetControls()
        {
            CheckBox activeOnly = (CheckBox)personsFilter.FindControl("chbShowActive");
            CheckBox projected = (CheckBox)personsFilter.FindControl("chbProjected");
            CheckBox terminated = (CheckBox)personsFilter.FindControl("chbTerminated");
            CheckBox terminatedPending = (CheckBox)personsFilter.FindControl("chbTerminationPending");

            personsFilter.ResetFilterControlsToDefault();
            SelectAllItems(this.cblRecruiters);

            activeOnly.Checked = terminatedPending.Checked = true;
            projected.Checked = terminated.Checked = false;
            txtSearch.Text = string.Empty;
            ddlView.SelectedValue = "-1";
        }

        private void ResetFilterControlsToDefault()
        {

            ResetControls();
            //Reset to All button.
            if (previousLetter != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLetter);

                LinkButton prevtopButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLinkButton.Attributes["Top"]);
                //LinkButton prevbottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(previousLinkButton.Attributes["Bottom"]);

                prevtopButton.CssClass = "fontNormal";

                //prevbottomButton.Font.Bold = false;
            }
            lnkbtnAll.CssClass = "fontBold";
            //lnkbtnAll1.Font.Bold = true;
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
                pageSize = GetPersonCount(PracticeIdsSelectedKey,
                                            DivisionIdsSelectedKey,
                                            Convert.ToBoolean(hdnActive.Value),
                                            0,
                                            1,
                                            hdnLooked.Value,
                                            RecruiterIdsSelectedKey,
                                            PayTypeIdsSelectedKey,
                                            Convert.ToBoolean(hdnProjected.Value),
                                            Convert.ToBoolean(hdnTerminated.Value),
                                            Convert.ToBoolean(hdnTerminatedPending.Value),
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

        public string GetHireDate(Person person)
        {
            if (person.IsStrawMan)
            {
                return string.Empty;
            }
            else
            {
                return FormatDate(person.HireDate);
            }
        }

        protected void odsPersons_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["practiceIdsSelected"] = PracticeIdsSelectedKey;
            e.InputParameters["DivisionIdsSelected"] = DivisionIdsSelectedKey;
            e.InputParameters["active"] = hdnActive.Value;
            e.InputParameters["pageSize"] = GetPageSize(ddlView.SelectedValue);
            e.InputParameters["pageNo"] = CurrentIndex;
            e.InputParameters["looked"] = hdnLooked.Value;
            e.InputParameters["recruitersSelected"] = RecruiterIdsSelectedKey;
            e.InputParameters["payTypeIdsSelected"] = PayTypeIdsSelectedKey;
            e.InputParameters["projected"] = hdnProjected.Value;
            e.InputParameters["terminated"] = hdnTerminated.Value;
            e.InputParameters["terminatedPending"] = hdnTerminatedPending.Value;
            e.InputParameters["alphabet"] = hdnAlphabet.Value;

        }
        #endregion

        private void SaveFilterValuesForSession()
        {
            PersonsFilters filter = new PersonsFilters();
            filter.SearchText = txtSearch.Text;
            filter.ReportView = ddlView.SelectedValue;
            filter.RecruiterIds = cblRecruiters.SelectedItems;
            filter.IsActive = personsFilter.Active;
            filter.IsContingent = personsFilter.Projected;
            filter.IsTerminated = personsFilter.Terminated;
            filter.IsTeminationPending = personsFilter.TerminationPending;
            filter.PayTypeIds = personsFilter.PayTypeIds;
            filter.PracticeAreaIds = personsFilter.PracticeIds;
            filter.DivisionIds = personsFilter.DivisionIds;
            ReportName reportName = HostingPageIsPersonReport != null ? ReportName.PersonSummaryReport : ReportName.PersonsReport;
            ReportsFilterHelper.SaveFilterValues(reportName, filter);
        }

        private void GetFilterValuesForSession()
        {
            ReportName reportName = HostingPageIsPersonReport != null ? ReportName.PersonSummaryReport : ReportName.PersonsReport;
            var filters = ReportsFilterHelper.GetFilterValues(reportName) as PersonsFilters;
            if (filters != null)
            {
                //personsFilter.Init;
                hdnLooked.Value = txtSearch.Text = filters.SearchText;
                ddlView.SelectedValue = filters.ReportView;
                RecruiterIdsSelectedKey = cblRecruiters.SelectedItems = filters.RecruiterIds;
                personsFilter.Active = filters.IsActive;
                hdnActive.Value = filters.IsActive.ToString();
                personsFilter.Projected = filters.IsContingent;
                hdnProjected.Value = filters.IsContingent.ToString();
                personsFilter.Terminated = filters.IsTerminated;
                hdnTerminated.Value = filters.IsTerminated.ToString();
                personsFilter.TerminationPending = filters.IsTeminationPending;
                hdnTerminatedPending.Value = filters.IsTeminationPending.ToString();
                PayTypeIdsSelectedKey = personsFilter.PayTypeIds = filters.PayTypeIds;
                PracticeIdsSelectedKey = personsFilter.PracticeIds = filters.PracticeAreaIds;
                DivisionIdsSelectedKey = personsFilter.DivisionIds = filters.DivisionIds;
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    SearchPersons();
                }
                else
                {
                    btnClearResults.Enabled = false;
                    hdnCleartoDefaultView.Value = "false";
                    txtSearch.Text = string.Empty;
                    CurrentIndex = 0;
                    SetFilterValues();
                    gvPersons.DataBind();
                }
            }
        }
        #endregion
    }
}
