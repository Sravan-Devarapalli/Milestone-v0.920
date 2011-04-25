using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Data;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.ServiceModel;
namespace PraticeManagement.Controls
{
    public partial class BudgetEntries : System.Web.UI.UserControl
    {

        #region Constants

        private const string ViewStateDirectorSortOrder = "DirectorSortOrder";
        private const string ViewStateDirectorSortDirection = "DirectorSortDirection";
        private const string ViewStateBDMSortOrder = "BDMSortOrder";
        private const string ViewStateBDMSortDirection = "BDMSortDirection";
        private const string ViewStatePracticeAreaSortOrder = "PracticeAreaSortOrder";
        private const string ViewStatePracticeAreaSortDirection = "PracticeAreaSortDirection";
        private const string CssArrowClass = "arrow";
        private const string grdDirectorBudgetEntriesId = "grdDirectorBudgetEntries";
        private const string grdBDMBudgetEntriesId = "grdBDMBudgetEntries";
        private const string grdPraticeAreaBugetEntriesId = "grdPraticeAreaBugetEntries";
        private const string groupedDirectors = "GroupedDirectors";
        private const string groupedBDManagers = "GroupedBDManagers";
        private const string groupedPracticeAreas = "GroupedPracticeAreas";

        #endregion

        #region Properties

        public ProjectsGroupedByPerson[] GroupedDirectors
        {
            get
            {
                var value = ViewState[groupedDirectors];

                if (value == null)
                {
                    int selectedYear = int.Parse(lblYear.Text);
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.ClientDirector);
                    }
                    ViewState[groupedDirectors] = value;
                }
                return (ProjectsGroupedByPerson[])value;
            }
            set
            {
                ViewState[groupedDirectors] = value;
            }
        }

        public ProjectsGroupedByPerson[] GroupedBDManagers
        {
            get
            {
                var value = ViewState[groupedBDManagers];

                if (value == null)
                {
                    int selectedYear = int.Parse(lblYear.Text);
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.BusinessDevelopmentManager);
                    }
                    ViewState[groupedBDManagers] = value;
                }
                return (ProjectsGroupedByPerson[])value;
            }
            set
            {
                ViewState[groupedBDManagers] = value;
            }
        }

        public ProjectsGroupedByPractice[] GroupedPracticeAreas
        {
            get
            {
                var value = ViewState[groupedPracticeAreas];

                if (value == null)
                {
                    int selectedYear = int.Parse(lblYear.Text);
                    using (var serviceClient = new ProjectService.ProjectServiceClient())
                    {
                        value = serviceClient.PracticeBudgetListByYear(selectedYear);
                    }
                    ViewState[groupedPracticeAreas] = value;
                }
                return (ProjectsGroupedByPractice[])value;
            }
            set
            {
                ViewState[groupedPracticeAreas] = value;
            }
        }

        private PraticeManagement.Financial HostingPage
        {
            get { return ((PraticeManagement.Financial)Page); }
        }

        protected SortDirection DirectorSortDirection
        {
            get
            {
                var value = ViewState[ViewStateDirectorSortDirection];

                return value == null ? SortDirection.Ascending : (SortDirection)value;
            }
            set
            {
                ViewState[ViewStateDirectorSortDirection] = value;
            }
        }

        protected string DirectorOrderBy
        {
            get
            {
                var value = ViewState[ViewStateDirectorSortOrder];
                return value == null ? null : value.ToString();
            }
            set
            {
                ViewState[ViewStateDirectorSortOrder] = value;
            }
        }

        protected SortDirection BDMSortDirection
        {
            get
            {
                var value = ViewState[ViewStateBDMSortDirection];

                return value == null ? SortDirection.Ascending : (SortDirection)value;
            }
            set
            {
                ViewState[ViewStateBDMSortDirection] = value;
            }
        }

        protected string BDMOrderBy
        {
            get
            {
                var value = ViewState[ViewStateBDMSortOrder];
                return value == null ? null : value.ToString();
            }
            set
            {
                ViewState[ViewStateBDMSortOrder] = value;
            }
        }

        protected SortDirection PracticeAreaSortDirection
        {
            get
            {
                var value = ViewState[ViewStatePracticeAreaSortDirection];

                return value == null ? SortDirection.Ascending : (SortDirection)value;
            }
            set
            {
                ViewState[ViewStatePracticeAreaSortDirection] = value;
            }
        }

        protected string PracticeAreaOrderBy
        {
            get
            {
                var value = ViewState[ViewStatePracticeAreaSortOrder];
                return value == null ? null : value.ToString();
            }
            set
            {
                ViewState[ViewStatePracticeAreaSortOrder] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int selectedYear = DateTime.Now.Year;
                lblYear.Text = selectedYear.ToString();
                ProjectsGroupedByPerson[] groupedDirectors;
                ProjectsGroupedByPerson[] groupedBDMs;
                ProjectsGroupedByPractice[] groupedPAs;
                using (var serviceClient = new ProjectService.ProjectServiceClient())
                {
                    groupedDirectors = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.ClientDirector);
                    groupedBDMs = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.BusinessDevelopmentManager);
                    groupedPAs = serviceClient.PracticeBudgetListByYear(selectedYear);
                }

                GroupedDirectors = groupedDirectors;
                grdDirectorBudgetEntries.DataSource = groupedDirectors;
                grdDirectorBudgetEntries.DataBind();

                GroupedBDManagers = groupedBDMs;
                grdBDMBudgetEntries.DataSource = groupedBDMs;
                grdBDMBudgetEntries.DataBind();

                GroupedPracticeAreas = groupedPAs;
                grdPraticeAreaBugetEntries.DataSource = groupedPAs;
                grdPraticeAreaBugetEntries.DataBind();
            }
        }

        public void LoadRevenueGoals()
        {
            grdDirectorBudgetEntries.DataSource = GroupedDirectors;
            grdDirectorBudgetEntries.DataBind();

            grdBDMBudgetEntries.DataSource = GroupedBDManagers;
            grdBDMBudgetEntries.DataBind();

            grdPraticeAreaBugetEntries.DataSource = GroupedPracticeAreas;
            grdPraticeAreaBugetEntries.DataBind();
        }

        public void SaveGoals_Clicked(object sender, EventArgs e)
        {
            if (ValidateAndSaveRevenueGoals())
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "alert('Goals saved successfully.');", true);
            }
        }

        internal bool ValidateAndSaveRevenueGoals()
        {
            bool saved = false;

            Page.Validate(valSummaryDirector.ValidationGroup);
            Page.Validate(valSummaryPracticeArea.ValidationGroup);
            Page.Validate(valSummaryBDM.ValidationGroup);
            if (Page.IsValid)
            {
                int year = int.Parse(lblYear.Text);

                List<CategoryItemBudget> categoryItemBudgetList = new List<CategoryItemBudget>();

                List<CategoryItemBudget> DirectorEntriesList = SaveDirectorEntries();
                List<CategoryItemBudget> PracticeAreaEntriesList = SavePracticeAreaEntries();
                List<CategoryItemBudget> BDMEntriesList = SaveBDMEntries();

                if (DirectorEntriesList.Count > 0)
                    categoryItemBudgetList.AddRange(DirectorEntriesList);
                if (PracticeAreaEntriesList.Count > 0)
                    categoryItemBudgetList.AddRange(PracticeAreaEntriesList);
                if (BDMEntriesList.Count > 0)
                    categoryItemBudgetList.AddRange(BDMEntriesList);

                using (var serviceClient = new ProjectService.ProjectServiceClient())
                {
                    try
                    {
                        serviceClient.CategoryItemsSaveFromXML(categoryItemBudgetList.ToArray(), year);
                        saved = true;

                        HostingPage.PageBaseClearDirty();
                        ViewState.Remove(groupedDirectors);
                        ViewState.Remove(groupedBDManagers);
                        ViewState.Remove(groupedPracticeAreas);
                    }
                    catch (CommunicationException ex)
                    {
                        serviceClient.Abort();
                    }
                }
            }
            return saved;
        }

        public void imgbtnNavigateYear_Click(object sender, EventArgs e)
        {
            if (HostingPage.IsDirty)
            {
                if (HostingPage.PageBaseSaveDirty)
                {
                    if (!ValidateAndSaveRevenueGoals())
                    {
                        return;
                    }
                }
                else
                {
                    LoadRevenueGoals();
                    HostingPage.PageBaseClearDirty();
                }
            }

            var button = sender as ImageButton;
            int selectedYear = int.Parse(lblYear.Text);
            if (button.ID == "imgbtnPrevious")
                selectedYear--;
            else
                selectedYear++;
            lblYear.Text = selectedYear.ToString();
            ProjectsGroupedByPerson[] groupedDirectors;
            ProjectsGroupedByPerson[] groupedBDMs;
            ProjectsGroupedByPractice[] groupedPAs;
            using (var serviceClient = new ProjectService.ProjectServiceClient())
            {
                groupedDirectors = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.ClientDirector);
                groupedBDMs = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.BusinessDevelopmentManager);
                groupedPAs = serviceClient.PracticeBudgetListByYear(selectedYear);
            }
            grdDirectorBudgetEntries.DataSource = groupedDirectors;
            grdDirectorBudgetEntries.DataBind();

            grdBDMBudgetEntries.DataSource = groupedBDMs;
            grdBDMBudgetEntries.DataBind();

            grdPraticeAreaBugetEntries.DataSource = groupedPAs;
            grdPraticeAreaBugetEntries.DataBind();
        }

        protected void BudgetEntries_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (HostingPage.IsDirty)
            {
                if (HostingPage.PageBaseSaveDirty)
                {
                    if (!ValidateAndSaveRevenueGoals())
                    {
                        return;
                    }
                }
                else
                {
                    LoadRevenueGoals();
                    HostingPage.PageBaseClearDirty();
                }
            }

            var gvBudgetEntries = (GridView)sender;
            var id = gvBudgetEntries.ID;
            var newOrder = e.SortExpression;

            int selectedYear = int.Parse(lblYear.Text);
            switch (id)
            {
                case grdDirectorBudgetEntriesId:
                    DirectorSorting(newOrder, selectedYear);
                    break;
                case grdBDMBudgetEntriesId:
                    BDMSorting(newOrder, selectedYear);
                    break;
                case grdPraticeAreaBugetEntriesId:
                    PracticeAreaSorting(newOrder, selectedYear);
                    break;
            }
        }

        protected void BudgetEntries_Sorted(object sender, EventArgs e)
        {
            if (HostingPage.IsDirty)
            {
                if (HostingPage.PageBaseSaveDirty)
                {
                    if (!ValidateAndSaveRevenueGoals())
                    {
                        return;
                    }
                }
                else
                {
                    HostingPage.PageBaseClearDirty();
                }
            }

            var gvBudgetEntries = (GridView)sender;
            string id = gvBudgetEntries.ID;
            TableRow headerRow = gvBudgetEntries.HeaderRow;

            if (headerRow.HasControls())
            {
                TableCell cell = headerRow.Cells[0];

                if (cell.HasControls())
                {
                    foreach (var ctrl in cell.Controls)
                    {
                        if (ctrl is LinkButton)
                        {
                            var lb = (LinkButton)ctrl;

                            lb.CssClass = CssArrowClass;

                            lb.Attributes["Width"] = "30%";
                            if (id == grdDirectorBudgetEntriesId)
                            {
                                lb.CssClass += string.Format(" sort-{0}", DirectorSortDirection == SortDirection.Ascending ? "up" : "down");
                            }
                            else if (id == grdBDMBudgetEntriesId)
                            {
                                lb.CssClass += string.Format(" sort-{0}", BDMSortDirection == SortDirection.Ascending ? "up" : "down");
                            }
                            else if (id == grdPraticeAreaBugetEntriesId)
                            {
                                lb.CssClass += string.Format(" sort-{0}", PracticeAreaSortDirection == SortDirection.Ascending ? "up" : "down");
                            }
                        }
                    }
                }
            }
        }

        protected void Amount_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            Decimal result;
            string amount = e.Value;
            bool isDecimal = Decimal.TryParse(amount, out result);

            e.IsValid = (isDecimal && result >= 0);

            var cv = (CustomValidator)sender;

            TableCell parent = (TableCell)cv.Parent;

            TextBox item = (TextBox)parent.FindControl(cv.ControlToValidate);

            if (item.Enabled)
            {
                item.BackColor = e.IsValid ? System.Drawing.Color.White : System.Drawing.Color.Red;
                item.ToolTip = e.IsValid ? string.Empty : "A positive number with 2 decimal digits is allowed for the Amount.";
            }
        }

        #endregion

        #region Methods

        private Dictionary<DateTime, ComputedFinancials> GetProjectedFinancials(DateTime monthBegin, int periodLength)
        {
            var projectedFinancials = new Dictionary<DateTime, ComputedFinancials>();

            for (int k = 0; k < periodLength; k++, monthBegin = monthBegin.AddMonths(1))
            {
                var computedFinancials = new ComputedFinancials();
                computedFinancials.FinancialDate = monthBegin;
                computedFinancials.Revenue += 0;
                computedFinancials.GrossMargin += 0;
                projectedFinancials.Add(monthBegin, computedFinancials);
            }
            return projectedFinancials;
        }

        public string GetMonthAmount(object projectedFinancials, int Month)
        {
            int selectedYear = int.Parse(lblYear.Text);
            var monthstartDate = new DateTime(selectedYear, Month, 1);
            var projectedFinancialsDict = projectedFinancials as Dictionary<DateTime, ComputedFinancials>;
            if (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthstartDate))
            {
                var financials = projectedFinancialsDict.First(kvp => kvp.Key == monthstartDate).Value;
                return financials.Revenue.Value.ToString();
            }
            return string.Empty;
        }

        public bool CheckMonthEnabled(object projectedFinancials, int Month)
        {
            int selectedYear = int.Parse(lblYear.Text);
            var monthstartDate = new DateTime(selectedYear, Month, 1);
            var projectedFinancialsDict = projectedFinancials as Dictionary<DateTime, ComputedFinancials>;
            return (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthstartDate));
        }

        public System.Drawing.Color GetBackColor(object projectedFinancials, int Month)
        {
            int selectedYear = int.Parse(lblYear.Text);
            var monthstartDate = new DateTime(selectedYear, Month, 1);
            var projectedFinancialsDict = projectedFinancials as Dictionary<DateTime, ComputedFinancials>;
            if (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthstartDate))
            {
                return System.Drawing.Color.White;
            }
            return System.Drawing.Color.FromArgb(204, 204, 204);
        }

        private void DirectorSorting(string newOrder, int selectedYear)
        {
            if (newOrder == DirectorOrderBy)
            {
                DirectorSortDirection =
                    DirectorSortDirection == SortDirection.Ascending ?
                        SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                DirectorOrderBy = newOrder;
                DirectorSortDirection = SortDirection.Ascending;
            }

            using (var serviceClient = new ProjectService.ProjectServiceClient())
            {
                if (DirectorSortDirection == SortDirection.Ascending)
                {
                    var dataSource = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.ClientDirector).OrderBy(i => i.LastName + ',' + i.FirstName);

                    grdDirectorBudgetEntries.DataSource = dataSource;
                    grdDirectorBudgetEntries.DataBind();
                }
                else
                {
                    var dataSource = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.ClientDirector).OrderByDescending(i => i.LastName + ',' + i.FirstName);

                    grdDirectorBudgetEntries.DataSource = dataSource;
                    grdDirectorBudgetEntries.DataBind();
                }
            }
        }

        private void BDMSorting(string newOrder, int selectedYear)
        {
            if (newOrder == BDMOrderBy)
            {
                BDMSortDirection =
                    BDMSortDirection == SortDirection.Ascending ?
                        SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                BDMOrderBy = newOrder;
                BDMSortDirection = SortDirection.Ascending;
            }

            using (var serviceClient = new ProjectService.ProjectServiceClient())
            {
                if (BDMSortDirection == SortDirection.Ascending)
                {
                    var dataSource = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.BusinessDevelopmentManager).OrderBy(i => i.LastName + ',' + i.FirstName);
                    grdBDMBudgetEntries.DataSource = dataSource;
                    grdBDMBudgetEntries.DataBind();
                }
                else
                {
                    var dataSource = serviceClient.PersonBudgetListByYear(selectedYear, BudgetCategoryType.BusinessDevelopmentManager).OrderByDescending(i => i.LastName + ',' + i.FirstName);
                    grdBDMBudgetEntries.DataSource = dataSource;
                    grdBDMBudgetEntries.DataBind();
                }
            }
        }

        private void PracticeAreaSorting(string newOrder, int selectedYear)
        {
            if (newOrder == PracticeAreaOrderBy)
            {
                PracticeAreaSortDirection =
                    PracticeAreaSortDirection == SortDirection.Ascending ?
                        SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                PracticeAreaOrderBy = newOrder;
                PracticeAreaSortDirection = SortDirection.Ascending;
            }

            using (var serviceClient = new ProjectService.ProjectServiceClient())
            {
                if (PracticeAreaSortDirection == SortDirection.Ascending)
                {
                    var dataSource = serviceClient.PracticeBudgetListByYear(selectedYear).OrderBy(i => i.Name);
                    grdPraticeAreaBugetEntries.DataSource = dataSource;
                    grdPraticeAreaBugetEntries.DataBind();
                }
                else
                {
                    var dataSource = serviceClient.PracticeBudgetListByYear(selectedYear).OrderByDescending(i => i.Name);
                    grdPraticeAreaBugetEntries.DataSource = dataSource;
                    grdPraticeAreaBugetEntries.DataBind();
                }
            }
        }

        //private bool ValidAmount(string amount)
        //{
        //    Decimal result;
        //    bool isDecimal = Decimal.TryParse(amount, out result);

        //    return (isDecimal && result >= 0);
        //}

        private List<CategoryItemBudget> SaveDirectorEntries()
        {
            List<CategoryItemBudget> categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (TableRow row in grdDirectorBudgetEntries.Rows)
            {
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    TableCell cell = row.Cells[i];
                    if (cell.HasControls())
                    {
                        TextBox ctrlTextBox = new TextBox();
                        HiddenField ctrlHiddenField = new HiddenField();

                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is TextBox)
                            {
                                ctrlTextBox = (TextBox)ctrl;
                            }
                            if (ctrl is HiddenField)
                            {
                                ctrlHiddenField = (HiddenField)ctrl;
                            }
                        }

                        if (ctrlTextBox != null && ctrlHiddenField != null)
                        {
                            var txtBudget = (TextBox)ctrlTextBox;
                            var hdntxtBudget = (HiddenField)ctrlHiddenField;
                            if (txtBudget.Enabled)
                            {
                                if (hdntxtBudget.Value == "true")
                                {
                                    if (string.IsNullOrEmpty(txtBudget.Text))
                                        txtBudget.Text = "0";
                                    CategoryItemBudget categoryItemBudget = new CategoryItemBudget();
                                    categoryItemBudget.ItemId = int.Parse(txtBudget.Attributes["PersonId"]);
                                    categoryItemBudget.Amount = decimal.Parse(txtBudget.Text);
                                    categoryItemBudget.Month = int.Parse(txtBudget.Attributes["MonthIndex"]);
                                    categoryItemBudget.CategoryTypeId = BudgetCategoryType.ClientDirector;

                                    categoryItemBudgetList.Add(categoryItemBudget);

                                    hdntxtBudget.Value = "false";
                                }
                            }
                        }
                    }
                }
            }
            return categoryItemBudgetList;
        }

        private List<CategoryItemBudget> SavePracticeAreaEntries()
        {
            List<CategoryItemBudget> categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (TableRow row in grdPraticeAreaBugetEntries.Rows)
            {
                string itemNode = string.Empty;
                int itemId = 0;
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    TableCell cell = row.Cells[i];
                    if (cell.HasControls())
                    {
                        TextBox ctrlTextBox = new TextBox();
                        HiddenField ctrlHiddenField = new HiddenField();

                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is TextBox)
                            {
                                ctrlTextBox = (TextBox)ctrl;
                            }
                            if (ctrl is HiddenField)
                            {
                                ctrlHiddenField = (HiddenField)ctrl;
                            }
                        }

                        if (ctrlTextBox != null && ctrlHiddenField != null)
                        {
                            var txtBudget = (TextBox)ctrlTextBox;
                            var hdntxtBudget = (HiddenField)ctrlHiddenField;
                            if (txtBudget.Enabled)
                            {
                                if (hdntxtBudget.Value == "true")
                                {
                                    if (string.IsNullOrEmpty(txtBudget.Text))
                                        txtBudget.Text = "0";
                                    CategoryItemBudget categoryItemBudget = new CategoryItemBudget();
                                    categoryItemBudget.ItemId = int.Parse(txtBudget.Attributes["PracticeId"]);
                                    categoryItemBudget.Amount = decimal.Parse(txtBudget.Text);
                                    categoryItemBudget.Month = int.Parse(txtBudget.Attributes["MonthIndex"]);
                                    categoryItemBudget.CategoryTypeId = BudgetCategoryType.PracticeArea;

                                    categoryItemBudgetList.Add(categoryItemBudget);

                                    hdntxtBudget.Value = "false";
                                }
                            }
                        }
                    }
                }
            }
            return categoryItemBudgetList;
        }

        private List<CategoryItemBudget> SaveBDMEntries()
        {
            List<CategoryItemBudget> categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (TableRow row in grdBDMBudgetEntries.Rows)
            {
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    TableCell cell = row.Cells[i];
                    if (cell.HasControls())
                    {
                        TextBox ctrlTextBox = new TextBox();
                        HiddenField ctrlHiddenField = new HiddenField();

                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is TextBox)
                            {
                                ctrlTextBox = (TextBox)ctrl;
                            }
                            if (ctrl is HiddenField)
                            {
                                ctrlHiddenField = (HiddenField)ctrl;
                            }
                        }

                        if (ctrlTextBox != null && ctrlHiddenField != null)
                        {
                            var txtBudget = (TextBox)ctrlTextBox;
                            var hdntxtBudget = (HiddenField)ctrlHiddenField;
                            if (txtBudget.Enabled)
                            {
                                if (hdntxtBudget.Value == "true")
                                {
                                    if (string.IsNullOrEmpty(txtBudget.Text))
                                        txtBudget.Text = "0";
                                    CategoryItemBudget categoryItemBudget = new CategoryItemBudget();
                                    categoryItemBudget.ItemId = int.Parse(txtBudget.Attributes["PersonId"]);
                                    categoryItemBudget.Amount = decimal.Parse(txtBudget.Text);
                                    categoryItemBudget.Month = int.Parse(txtBudget.Attributes["MonthIndex"]);
                                    categoryItemBudget.CategoryTypeId = BudgetCategoryType.BusinessDevelopmentManager;

                                    categoryItemBudgetList.Add(categoryItemBudget);

                                    hdntxtBudget.Value = "false";
                                }
                            }
                        }
                    }
                }
            }
            return categoryItemBudgetList;
        }

        #endregion
    }
}

