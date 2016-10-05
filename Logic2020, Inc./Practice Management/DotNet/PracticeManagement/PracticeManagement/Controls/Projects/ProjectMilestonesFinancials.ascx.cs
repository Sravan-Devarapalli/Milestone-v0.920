using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using PraticeManagement.MilestoneService;
using System.ServiceModel;
using System.Collections.Generic;
using PraticeManagement.ProjectService;
using System.Linq;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectMilestonesFinancials : UserControl
    {
        #region Delegates

        public delegate int SaveDataDelegate();

        public delegate bool ValidateAndSaveDelegate();

        #endregion Delegates

        #region Constants

        public const string MILESTONE_TARGET = "milestone";
        protected const string ProjectIdFormat = "projectId={0}";

        private const int GROSS_MARGIN_CELL_INDEX = 5;
        private const int MARGIN_PERCENT_CELL_INDEX = 6;

        private const string ViewSortExpression = "SortExpression";
        private const string ViewSortDirection = "SortDirection";

        private const string CssArrowClass = "arrow";

        private const string WordBreak = "<wbr />";

        #endregion Constants

        private SeniorityAnalyzer milestonesSeniorityAnalyzer;

        #region Properties

        public bool IsAttributionPanelDisplayed
        {
            get
            {
                if (ViewState["IsAttributionPanelDisplayed_Key"] == null)
                {
                    ViewState["IsAttributionPanelDisplayed_Key"] = false;
                }

                return (bool)ViewState["IsAttributionPanelDisplayed_Key"];
            }
            set
            {
                ViewState["IsAttributionPanelDisplayed_Key"] = value;
            }
        }

        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public List<int> MilestoneCSATAttributionCount
        {
            get
            {
                using (var serviceClient = new MilestoneServiceClient())
                {
                    try
                    {
                        return serviceClient.GetMilestoneAndCSATCountsByProject(HostingPage.ProjectId.Value).ToList();
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
        }

        protected int? ProjectId
        {
            get
            {
                try
                {
                    return HostingPage.ProjectId;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private string PreviousSortExpression
        {
            get
            {
                return (string)ViewState[ViewSortExpression];
            }
            set
            {
                ViewState[ViewSortExpression] = value;
            }
        }

        private SortDirection PreviousSortDirection
        {
            get
            {
                var value = ViewState[ViewSortDirection];
                return value == null ? SortDirection.Ascending : (SortDirection)value;
            }
            set
            {
                ViewState[ViewSortDirection] = value;
            }
        }

        public int MilestoneId
        {
            get
            {
                if (ViewState["MilestoneId_Key"] == null)
                {
                    ViewState["MilestoneId_Key"] = -1;
                }
                return (int)ViewState["MilestoneId_Key"];
            }
            set
            {
                ViewState["MilestoneId_Key"] = value;
            }
        }

        public string ValidationGroup { get; set; }

        #endregion Properties

        protected void Page_Load(object sender, EventArgs e)
        {
            milestonesSeniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
            if (!IsPostBack)
            {
                PopulateData();
            }
        }

        protected void imgbtnUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as RepeaterItem;
            var tbMilesonename = row.FindControl("tbMilestoneName") as TextBox;
            var rfvMilestoneName = row.FindControl("rfvMilestoneName") as RequiredFieldValidator;
            rfvMilestoneName.ValidationGroup = ValidationGroup;
            rfvMilestoneName.Validate();
            if (!rfvMilestoneName.IsValid)
                return;
            var milestone = new Milestone
            {
                Id = Convert.ToInt32(tbMilesonename.Attributes["MilestoneId"]),
                Description = tbMilesonename.Text
            };
            //Save Milestone Name.
            ServiceCallers.Custom.Milestone(
                m => m.MilestoneUpdateShortDetails(milestone, HttpContext.Current.User.Identity.Name));
            imgEdit.CommandName = "";
            PopulateData();
        }

        protected void cvAttributionPopup_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (IsAttributionPanelDisplayed)
                return;
            var milestone = GetMilestoneById(MilestoneId);
            var attributionList = new List<Attribution>();
            var extendAttributionDates = new List<bool>();
            using (var service = new MilestoneServiceClient())
            {
                if (milestone != null)
                {
                    attributionList =
                        service.IsProjectAttributionConflictsWithMilestoneChanges(MilestoneId,
                                                                                  milestone.StartDate,
                                                                                  milestone.EndDate,
                                                                                  hdnIsUpdate.Value == true.ToString()).ToList();

                }
            }
            if (attributionList.Any())
            {
                trAttributionRecord.Visible = true;
                IsAttributionPanelDisplayed = true;
                mpeAttribution.Show();
                HostingPage.IsOtherPanelDisplay = true;
                if (attributionList.Any(x => x.AttributionType == AttributionTypes.Delivery))
                {
                    repDeliveryPersons.DataSource =
                        attributionList.Where(x => x.AttributionType == AttributionTypes.Delivery)
                                       .OrderBy(x => x.TargetName)
                                       .ThenBy(x => x.StartDate);
                    repDeliveryPersons.DataBind();
                }
                if (attributionList.Any(x => x.AttributionType == AttributionTypes.Sales))
                {
                    repSalesPersons.DataSource =
                        attributionList.Where(x => x.AttributionType == AttributionTypes.Sales)
                                       .OrderBy(x => x.TargetName)
                                       .ThenBy(x => x.StartDate);
                    repSalesPersons.DataBind();
                }
                e.IsValid = false;
            }
            else
                trAttributionRecord.Visible = false;

            if (hdnIsUpdate.Value != true.ToString())
            {
                trCommissionsStartDateExtend.Visible = false;
                trCommissionsEndDateExtend.Visible = false;
            }
        }

        public void MilestoneValidate(RepeaterItem row)
        {
            var custExpenseValidate = row.FindControl("custExpenseValidate") as CustomValidator;
            var custProjectStatus = row.FindControl("custProjectStatus") as CustomValidator;
            var custCSATValidate = row.FindControl("custCSATValidate") as CustomValidator;
            var custAttribution = row.FindControl("custAttribution") as CustomValidator;
            var custFeedback = row.FindControl("custFeedback") as CustomValidator;
            custExpenseValidate.Validate();
            custProjectStatus.Validate();
            custCSATValidate.Validate();
            custAttribution.Validate();
            custFeedback.Validate();
        }

        protected void btnOkAttribution_Click(object sender, EventArgs e)
        {
            if (hdnIsUpdate.Value == false.ToString())
                CallAppropriateDelete();
        }

        public void CallAppropriateDelete()
        {
            foreach (RepeaterItem row in repMilestones.Items)
            {
                var imgDeleteBtn = row.FindControl("imgMilestoneDelete") as ImageButton;
                int milestoneId;
                int.TryParse(imgDeleteBtn.Attributes["MilestoneId"], out milestoneId);
                if (milestoneId == MilestoneId)
                {
                    imgMilestoneDelete_Click(imgDeleteBtn, new EventArgs());
                    break;
                }
            }
        }

        protected void btnCancelAttribution_Click(object sender, EventArgs e)
        {
            IsAttributionPanelDisplayed = false;
            mpeAttribution.Hide();
        }

        protected void imgMilestoneDelete_Click(object sender, EventArgs e)
        {
            var imgDeletebtn = sender as ImageButton;
            var row = imgDeletebtn.NamingContainer as RepeaterItem;
            var btnDelete = row.FindControl("imgMilestoneDelete") as ImageButton;
            int milestoneId;
            int.TryParse(btnDelete.Attributes["MilestoneId"], out milestoneId);
            MilestoneId = milestoneId;
            MilestoneValidate(row);
            if (Page.IsValid)
            {
                hdnIsUpdate.Value = false.ToString();
                Page.Validate("AttributionPopup");
            }
            else
            {
                mpeErrorPanel.Show();
                HostingPage.IsOtherPanelDisplay = true;
            }
            if (Page.IsValid)
            {
                try
                {
                    DeleteRecord(milestoneId);
                    IsAttributionPanelDisplayed = false;
                    Response.Redirect(HostingPage.Page.Request.RawUrl);
                }
                catch (Exception exception)
                {
                    lblError.ShowErrorMessage("{0}", exception.Message);
                }
            }
            else
            {
                HostingPage.IsOtherPanelDisplay = true;
            }
        }

        private void DeleteRecord(int milestoneId)
        {
            var milestone = new Milestone { Id = (int?)milestoneId };

            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    serviceClient.DeleteMilestone(milestone, HostingPage.User.Identity.Name);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public Milestone GetMilestoneById(int id)
        {
            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    return serviceClient.GetMilestoneDetail(id);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void custExpenseValidate_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custExpense = sender as CustomValidator;
            var row = custExpense.NamingContainer as RepeaterItem;
            var btnDelete = row.FindControl("imgMilestoneDelete") as ImageButton;
            int milestoneId;
            int.TryParse(btnDelete.Attributes["MilestoneId"], out milestoneId);
            var milestone = GetMilestoneById(milestoneId);
            if (milestone.Project.StartDate.Value == milestone.StartDate ||
                milestone.Project.EndDate.Value == milestone.EndDate)
            {
                using (var service = new MilestoneServiceClient())
                {
                    if (service.CheckIfExpensesExistsForMilestonePeriod(milestoneId, null, null))
                    {
                        e.IsValid = false;
                    }
                    else
                    {
                        e.IsValid = true;
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        protected void custProjectStatus_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = (HostingPage.Project.Status.StatusType == ProjectStatusType.Active && MilestoneCSATAttributionCount[0] == 1) ? false : true;
        }

        protected void custCSATValidate_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = (MilestoneCSATAttributionCount[0] != 1 || MilestoneCSATAttributionCount[1] <= 0);
        }

        protected void custAttribution_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = (MilestoneCSATAttributionCount[0] != 1 || MilestoneCSATAttributionCount[2] <= 0);
        }

        protected void custFeedback_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            var custFeedback = sender as CustomValidator;
            var row = custFeedback.NamingContainer as RepeaterItem;
            var btnDelete = row.FindControl("imgMilestoneDelete") as ImageButton;
            int milestoneId;
            int.TryParse(btnDelete.Attributes["MilestoneId"], out milestoneId);
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    e.IsValid = !serviceClient.CheckIfFeedbackExists(null, milestoneId,null,null);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void imgbtnCancel_OnClick(object sender, EventArgs e)
        {
            PopulateData();
        }

        protected string GetMilestoneRedirectUrl(object milestoneId)
        {
            return Urls.GetMilestoneRedirectUrl(milestoneId, Request.Url.AbsoluteUri.Replace("&CSAT=true", ""), ProjectId.Value);
        }

        private static void HideCell(RepeaterItemEventArgs e, int cellIndex)
        {
            // This code is applicable only if the cell contains label in it.
            var lblEstimatedMargin = e.Item.FindControl("lblEstimatedMargin") as Label;
            var lblTargetMargin = e.Item.FindControl("lblTargetMargin") as Label;
            lblTargetMargin.Text = lblEstimatedMargin.Text = Resources.Controls.HiddenCellText;
            lblEstimatedMargin.CssClass = Convert.ToBoolean(lblEstimatedMargin.Attributes["NegativeValue"]) ? Resources.Controls.BenchCssClass : Resources.Controls.MarginCssClass;
        }

        public string GetWrappedTest(string text)
        {
            if (text.Length > 30)
            {
                text = text.Insert(30, WordBreak);
            }
            return text;
        }

        public void PopulateData()
        {
            using (var client = new ProjectServiceClient())
            {
                if (!ProjectId.HasValue) return;
                var data = client.GetProjectMilestonesFinancials(ProjectId.Value);
                if (data.Tables[0].Rows.Count > 0)
                {
                    repMilestones.DataSource = data;
                    repMilestones.DataBind();
                    divEmptyMessage.Style["display"] = "none";
                    return;
                }
                divEmptyMessage.Style["display"] = "";
            }
        }

        protected void repMilestones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var imgbtnUpdate = e.Item.FindControl("imgbtnUpdate");
            var imgbtnCancel = e.Item.FindControl("imgbtnCancel");
            var tbMilestoneName = e.Item.FindControl("tbMilestoneName");
            var hlMilestoneName = e.Item.FindControl("hlMilestoneName");
            var imgMilestoneDelete = e.Item.FindControl("imgMilestoneDelete");
            var imgbtnEdit = e.Item.FindControl("imgbtnEdit");
            if (e.CommandName == "edit")
            {
                imgbtnUpdate.Visible = imgbtnCancel.Visible = tbMilestoneName.Visible = true;
                hlMilestoneName.Visible = imgMilestoneDelete.Visible = imgbtnEdit.Visible = false;
                return;
            }
            imgbtnUpdate.Visible = imgbtnCancel.Visible = tbMilestoneName.Visible = false;
            hlMilestoneName.Visible = imgMilestoneDelete.Visible = imgbtnEdit.Visible = true;
        }

        protected void repMilestones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                HostingPage.noMileStones = true;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HostingPage.noMileStones = false;
                var imgMilestoneDelete = e.Item.FindControl("imgMilestoneDelete") as ImageButton;
                int milestoneId;
                int.TryParse(imgMilestoneDelete.Attributes["MilestoneId"], out milestoneId);
                var milestone = new Milestone { Id = milestoneId };
                if (milestonesSeniorityAnalyzer.OneWithGreaterSeniorityExists(
                    DataHelper.GetPersonsInMilestone(milestone)))
                {
                    HideCell(e, GROSS_MARGIN_CELL_INDEX);
                    HideCell(e, MARGIN_PERCENT_CELL_INDEX);
                }
            }
        }
    }
}

