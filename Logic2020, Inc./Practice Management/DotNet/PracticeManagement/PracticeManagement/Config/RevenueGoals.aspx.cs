using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
namespace PraticeManagement.Config
{
    public partial class RevenueGoals : PracticeManagementPageBase, IPostBackEventHandler
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

        protected override void Display()
        {
        }

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
                repDirectors.DataSource = groupedDirectors;
                repDirectors.DataBind();

                GroupedBDManagers = groupedBDMs;
                repBDMBudgetEntries.DataSource = groupedBDMs;
                repBDMBudgetEntries.DataBind();

                GroupedPracticeAreas = groupedPAs;
                repPracticeBudgetEntries.DataSource = groupedPAs;
                repPracticeBudgetEntries.DataBind();
            }
        }

        public void LoadRevenueGoals()
        {
            repDirectors.DataSource = GroupedDirectors;
            repDirectors.DataBind();

            repBDMBudgetEntries.DataSource = GroupedBDManagers;
            repBDMBudgetEntries.DataBind();

            repPracticeBudgetEntries.DataSource = GroupedPracticeAreas;
            repPracticeBudgetEntries.DataBind();
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

                var categoryItemBudgetList = new List<CategoryItemBudget>();

                var DirectorEntriesList = SaveDirectorEntries();
                var PracticeAreaEntriesList = SavePracticeAreaEntries();
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

                        this.ClearDirty();
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
            if (this.IsDirty)
            {
                if (this.SaveDirty)
                {
                    if (!ValidateAndSaveRevenueGoals())
                    {
                        return;
                    }
                }
                else
                {
                    LoadRevenueGoals();
                    this.ClearDirty();
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
            repDirectors.DataSource = groupedDirectors;
            repDirectors.DataBind();

            repBDMBudgetEntries.DataSource = groupedBDMs;
            repBDMBudgetEntries.DataBind();

            repPracticeBudgetEntries.DataSource = groupedPAs;
            repPracticeBudgetEntries.DataBind();
        }

        protected void Amount_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            Decimal result;
            string amount = e.Value;
            bool isDecimal = Decimal.TryParse(amount, out result);

            e.IsValid = (isDecimal && result >= 0);

            var cv = (CustomValidator)sender;

            var parent = (RepeaterItem)cv.Parent;

            var item = (TextBox)parent.FindControl(cv.ControlToValidate);

            if (item.Enabled)
            {
                item.BackColor = e.IsValid ? System.Drawing.Color.White : System.Drawing.Color.Red;
                item.ToolTip = e.IsValid ? string.Empty : "A positive number with 2 decimal digits is allowed for the Amount.";
            }
        }

        #endregion

        #region Methods

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

        public string GetBackColor(object projectedFinancials, int Month)
        {
            int selectedYear = int.Parse(lblYear.Text);
            var monthstartDate = new DateTime(selectedYear, Month, 1);
            var projectedFinancialsDict = projectedFinancials as Dictionary<DateTime, ComputedFinancials>;
            if (projectedFinancialsDict != null && projectedFinancialsDict.Any(kvp => kvp.Key == monthstartDate))
            {
                return "bgColorWhite";
            }
            return "bgcolor_CCCCCC";
        }

        private List<CategoryItemBudget> SaveDirectorEntries()
        {
            var categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (RepeaterItem row in repDirectors.Items)
            {
                var ctrlTextBox = new TextBox();
                HiddenField ctrlHiddenField;
                foreach (var ctrl in row.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ctrlTextBox = (TextBox)ctrl;
                    }
                    if (ctrl is HiddenField)
                    {
                        ctrlHiddenField = (HiddenField)ctrl;
                        var directorItem = GetDirectorBudgetItem(ctrlTextBox, ctrlHiddenField);
                        if (directorItem != null) categoryItemBudgetList.Add(directorItem);
                    }
                }
            }
            return categoryItemBudgetList;
        }

        private CategoryItemBudget GetDirectorBudgetItem(TextBox ctrlTextBox,HiddenField ctrlHiddenField)
        {
            if (ctrlTextBox == null || ctrlHiddenField == null) return null;
            var txtBudget = ctrlTextBox;
            var hdntxtBudget = ctrlHiddenField;
            if (txtBudget.Enabled)
            {
                if (hdntxtBudget.Value == "true")
                {
                    if (string.IsNullOrEmpty(txtBudget.Text))
                        txtBudget.Text = "0";
                    var categoryItemBudget = new CategoryItemBudget();
                    categoryItemBudget.ItemId = int.Parse(txtBudget.Attributes["PersonId"]);
                    categoryItemBudget.Amount = decimal.Parse(txtBudget.Text);
                    categoryItemBudget.Month = int.Parse(txtBudget.Attributes["MonthIndex"]);
                    categoryItemBudget.CategoryTypeId = BudgetCategoryType.ClientDirector;
                    hdntxtBudget.Value = "false";
                    return categoryItemBudget;
                }
            }
            return null;
        }

        private List<CategoryItemBudget> SavePracticeAreaEntries()
        {
            var categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (RepeaterItem row in repPracticeBudgetEntries.Items)
            {
                var ctrlTextBox = new TextBox();
                HiddenField ctrlHiddenField;
                foreach (var ctrl in row.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ctrlTextBox = (TextBox)ctrl;
                    }
                    if (ctrl is HiddenField)
                    {
                        ctrlHiddenField = (HiddenField)ctrl;
                        var practiceItem = GetPracticeBudgetItem(ctrlTextBox, ctrlHiddenField);
                        if (practiceItem != null) categoryItemBudgetList.Add(practiceItem);
                    }
                }
            }
            return categoryItemBudgetList;
        }

        private CategoryItemBudget GetPracticeBudgetItem(TextBox ctrlTextBox, HiddenField ctrlHiddenField)
        {
            if (ctrlTextBox == null || ctrlHiddenField == null) return null;
            var txtBudget = ctrlTextBox;
            var hdntxtBudget = ctrlHiddenField;
            if (txtBudget.Enabled)
            {
                if (hdntxtBudget.Value == "true")
                {
                    if (string.IsNullOrEmpty(txtBudget.Text))
                        txtBudget.Text = "0";
                    var categoryItemBudget = new CategoryItemBudget
                    {
                        ItemId = int.Parse(txtBudget.Attributes["PracticeId"]),
                        Amount = decimal.Parse(txtBudget.Text),
                        Month = int.Parse(txtBudget.Attributes["MonthIndex"]),
                        CategoryTypeId = BudgetCategoryType.PracticeArea
                    };
                    hdntxtBudget.Value = "false";
                    return categoryItemBudget;
                }
            }
            return null;
        }

        private List<CategoryItemBudget> SaveBDMEntries()
        {
            var categoryItemBudgetList = new List<CategoryItemBudget>();
            foreach (RepeaterItem row in repBDMBudgetEntries.Items)
            {
                var ctrlTextBox = new TextBox();
                HiddenField ctrlHiddenField;
                foreach (var ctrl in row.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ctrlTextBox = (TextBox)ctrl;
                    }
                    if (ctrl is HiddenField)
                    {
                        ctrlHiddenField = (HiddenField)ctrl;
                        var bdMItem = GetBDMBudgetItem(ctrlTextBox, ctrlHiddenField);
                        if (bdMItem != null) categoryItemBudgetList.Add(bdMItem);
                    }
                }
            }
            return categoryItemBudgetList;
        }

        private CategoryItemBudget GetBDMBudgetItem(TextBox ctrlTextBox, HiddenField ctrlHiddenField)
        {
            if (ctrlTextBox == null || ctrlHiddenField == null) return null;
            var txtBudget = ctrlTextBox;
            var hdntxtBudget = ctrlHiddenField;
            if (txtBudget.Enabled)
            {
                if (hdntxtBudget.Value == "true")
                {
                    if (string.IsNullOrEmpty(txtBudget.Text))
                        txtBudget.Text = "0";
                    var categoryItemBudget = new CategoryItemBudget
                    {
                        ItemId = int.Parse(txtBudget.Attributes["PersonId"]),
                        Amount = decimal.Parse(txtBudget.Text),
                        Month = int.Parse(txtBudget.Attributes["MonthIndex"]),
                        CategoryTypeId = BudgetCategoryType.BusinessDevelopmentManager
                    };
                    hdntxtBudget.Value = "false";
                    return categoryItemBudget;
                }
            }
            return null;
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (ValidateAndSaveRevenueGoals())
            {
                Redirect(eventArgument);
            }
        }
        #endregion
    }
}
