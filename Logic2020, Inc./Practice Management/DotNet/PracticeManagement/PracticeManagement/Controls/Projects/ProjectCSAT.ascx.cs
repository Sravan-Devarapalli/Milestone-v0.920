using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using DataTransferObjects;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectCSAT : System.Web.UI.UserControl
    {
        public const string ProjectCSAT_TARGET = "ProjectCsat";

        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public List<DataTransferObjects.ProjectCSAT> ProjectCSATList
        {
            get
            {
                if (ViewState["ProjectCSATList_Key"] == null)
                {
                    if (HostingPage.ProjectId.HasValue)
                    {
                        var list = ServiceCallers.Custom.Project(p => p.CSATList(HostingPage.ProjectId.Value));
                        ViewState["ProjectCSATList_Key"] = list;
                    }
                    else
                    {
                        return null;
                    }
                }
                return ((IEnumerable<DataTransferObjects.ProjectCSAT>)ViewState["ProjectCSATList_Key"]).ToList();
            }
            set
            {
                ViewState["ProjectCSATList_Key"] = value;
            }
        }

        private bool userIsAdministrator = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            ShowCSATButtons();
        }

        public void ShowCSATButtons()
        {
            stbCSAT.Visible =
            HostingPage.CSATTabEditPermission && (HostingPage.SelectedStatus == (int)ProjectStatusType.Active || (HostingPage.SelectedStatus == (int)ProjectStatusType.Completed && HostingPage.Project.Milestones.Count > 0));
        }

        public void PopulateData(List<DataTransferObjects.ProjectCSAT> projectCSATList)
        {
            ProjectCSATList = projectCSATList;
            gvCSAT.DataSource = ProjectCSATList;
            HostingPage.PopulateDirectorsList(ProjectCSATList);
            gvCSAT.DataBind();
        }

        protected void custCSATEndDateInGridView_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            CustomValidator custCSATCompletionDate = (CustomValidator)sender;
            GridViewRow row = custCSATCompletionDate.NamingContainer as GridViewRow;
            DatePicker dpReviewEndDate = (DatePicker)row.FindControl("dpReviewEndDate");
            if (HostingPage.Project.Status.StatusType == ProjectStatusType.Completed)
            {
                DateTime lastCompletedDate = ServiceCallers.Custom.Project(p => p.GetProjectLastChangeDateFortheGivenStatus(HostingPage.Project.Id.Value, HostingPage.Project.Status.Id));
                e.IsValid = dpReviewEndDate.DateValue.Date <= lastCompletedDate.Date;
            }
        }

        protected void custCSATCompleteDateInGridView_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custCSATCompleteDateInGridView = (CustomValidator)sender;
            GridViewRow row = custCSATCompleteDateInGridView.NamingContainer as GridViewRow;
            DatePicker dpCompletionDate = (DatePicker)row.FindControl("dpCompletionDate");
            e.IsValid = dpCompletionDate.DateValue.Date <= DateTime.Today.Date ? true : false;
        }

        protected void stbCSAT_Click(object sender, EventArgs e)
        {
            if (HostingPage.ProjectId.HasValue && HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                HostingPage.Redirect(Constants.ApplicationPages.ProjectCSATDetails + ProjectDetail.GetProjectIdArgument(false, HostingPage.ProjectId.Value));
            }
        }

        protected void gvCSAT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == gvCSAT.EditIndex)
                {
                    DataTransferObjects.ProjectCSAT dataItem = e.Row.DataItem as DataTransferObjects.ProjectCSAT;
                    DatePicker dpReviewStartDate = (DatePicker)e.Row.FindControl("dpReviewStartDate");
                    DatePicker dpReviewEndDate = (DatePicker)e.Row.FindControl("dpReviewEndDate");
                    DatePicker dpCompletionDate = (DatePicker)e.Row.FindControl("dpCompletionDate");
                    DropDownList ddlReviewer = (DropDownList)e.Row.FindControl("ddlReviewer");
                    DropDownList ddlScore = (DropDownList)e.Row.FindControl("ddlScore");
                    dpReviewStartDate.DateValue = dataItem.ReviewStartDate;
                    dpReviewEndDate.DateValue = dataItem.ReviewEndDate;
                    dpCompletionDate.DateValue = dataItem.CompletionDate;
                    BindScoreDropDown(ddlScore);
                    ddlScore.SelectedValue = dataItem.ReferralScore.ToString();
                    List<int> excludedPerson = new List<int>();
                    if (HostingPage.Project.Director != null && dataItem.ReviewerId != HostingPage.Project.Director.Id.Value)
                    {
                        excludedPerson.Add(HostingPage.Project.Director.Id.Value);
                    }
                    DataHelper.FillCSATReviewerList(ddlReviewer, "-- Select CSAT Reviewer --", excludedPerson);
                    ddlReviewer.SelectedValue = dataItem.ReviewerId.ToString();
                }
                else
                {
                    ImageButton imgDeleteCSAT = (ImageButton)e.Row.FindControl("imgDeleteCSAT");
                    HyperLink hlReviewStartDate = (HyperLink)e.Row.FindControl("hlReviewStartDate");
                    Label lblReviewStartDate = (Label)e.Row.FindControl("lblReviewStartDate");
                    imgDeleteCSAT.Visible = userIsAdministrator;
                    ImageButton imgEditCSAT = (ImageButton)e.Row.FindControl("imgEditCSAT");
                    imgEditCSAT.Visible = hlReviewStartDate.Visible = HostingPage.CSATTabEditPermission && (HostingPage.SelectedStatus == (int)ProjectStatusType.Active || HostingPage.SelectedStatus == (int)ProjectStatusType.Completed);
                    lblReviewStartDate.Visible = !imgEditCSAT.Visible;
                }
            }
        }

        //When client director is changed reviewer drop down repopulates.
        public void RePopulateReviewerDropdown()
        {
            if (gvCSAT.EditIndex > -1)
            {
                DropDownList ddlReviewer = (DropDownList)gvCSAT.Rows[gvCSAT.EditIndex].FindControl("ddlReviewer");
                string selectedReviewer = ddlReviewer.SelectedValue;
                DataTransferObjects.ProjectCSAT dataItem = ProjectCSATList[gvCSAT.Rows[gvCSAT.EditIndex].DataItemIndex] as DataTransferObjects.ProjectCSAT;
                List<int> excludedPerson = new List<int>();
                if (HostingPage.SelectedDirector != -1 && dataItem.ReviewerId != HostingPage.SelectedDirector)
                {
                    excludedPerson.Add(HostingPage.SelectedDirector);
                }
                DataHelper.FillCSATReviewerList(ddlReviewer, "-- Select Reviewer --", excludedPerson);
                ddlReviewer.SelectedValue = selectedReviewer;

            }

        }

        protected void ddlReviewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlReviewer = sender as DropDownList;
            var gvCSATItem = ddlReviewer.NamingContainer as GridViewRow;
            HiddenField hdCSATId = (HiddenField)gvCSATItem.FindControl("hdCSATId");
            int csatId = int.Parse(hdCSATId.Value);
            var temp = ProjectCSATList;
            var selectedCSAT = temp.Any(p => p.Id == csatId) ? temp.First(p => p.Id == csatId) : null;
            if (selectedCSAT != null)
                selectedCSAT.ReviewerId = !string.IsNullOrEmpty(ddlReviewer.SelectedValue) ? int.Parse(ddlReviewer.SelectedValue) : -1;
            HostingPage.PopulateDirectorsList(temp);
        }

        protected void btnReviewStartDate_Command(object sender, CommandEventArgs e)
        {
            if (HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                var selectedCSATId = Convert.ToInt32(e.CommandArgument);
            }
        }

        protected string GetProjectCSATDetailRedirectUrl(int cSATId)
        {
            string projectCSATPageurl = string.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectCSATDetails, cSATId) + ProjectDetail.GetProjectIdArgument(true, HostingPage.ProjectId.Value);
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(projectCSATPageurl, string.Format(Constants.ApplicationPages.DetailRedirectWithReturnFormat, Constants.ApplicationPages.ProjectDetail, HostingPage.ProjectId.Value + "&CSAT=true", Constants.ApplicationPages.Projects));
        }

        protected void imgEditCSAT_OnClick(object sender, EventArgs e)
        {
            if (HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                var imgEdit = sender as ImageButton;
                var gvCSATItem = imgEdit.NamingContainer as GridViewRow;
                gvCSAT.EditIndex = gvCSATItem.DataItemIndex;
                PopulateData(ProjectCSATList);
            }
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvCSAT.EditIndex = -1;
            ProjectCSATList = null;
            PopulateData(ProjectCSATList);
            HostingPage.PopulateDirectorsList(ProjectCSATList);
        }

        protected void imgUpdateCSAT_OnClick(object sender, EventArgs e)
        {
            var imgUpdate = sender as ImageButton;
            var row = imgUpdate.NamingContainer as GridViewRow;

            Page.Validate("CSATUpdate");
            if (Page.IsValid)
            {
                List<DataTransferObjects.ProjectCSAT> tmp = ProjectCSATList;
                int cSATId = int.Parse(((HiddenField)row.FindControl("hdCSATId")).Value);
                DatePicker dpReviewStartDate = (DatePicker)row.FindControl("dpReviewStartDate");
                DatePicker dpReviewEndDate = (DatePicker)row.FindControl("dpReviewEndDate");
                DatePicker dpCompletionDate = (DatePicker)row.FindControl("dpCompletionDate");
                DropDownList ddlReviewer = (DropDownList)row.FindControl("ddlReviewer");
                DropDownList ddlScore = (DropDownList)row.FindControl("ddlScore");

                DataTransferObjects.ProjectCSAT pCSAT = tmp.First(g => g.Id == cSATId);
                pCSAT.ReferralScore = int.Parse(ddlScore.SelectedValue);
                pCSAT.ReviewStartDate = dpReviewStartDate.DateValue;
                pCSAT.ReviewEndDate = dpReviewEndDate.DateValue;
                pCSAT.CompletionDate = dpCompletionDate.DateValue;
                pCSAT.ReviewerId = int.Parse(ddlReviewer.SelectedValue);

                ServiceCallers.Custom.Project(p => p.CSATUpdate(pCSAT, DataHelper.CurrentPerson.Alias));
                HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT successfully updated.");
                ProjectCSATList = null;
                tmp = ProjectCSATList;
                HostingPage.ClearDirtyForChildControls();
                gvCSAT.EditIndex = -1;
                PopulateData(tmp);
                HostingPage.IsErrorPanelDisplay = true;
            }
        }

        protected void imgDeleteCSAT_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var row = imgDelete.NamingContainer as GridViewRow;
            List<DataTransferObjects.ProjectCSAT> tmp = ProjectCSATList;
            int cSATId = int.Parse(((HiddenField)row.FindControl("hdCSATId")).Value);

            bool userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

            if (tmp.Any(g => g.Id == cSATId) && userIsAdministrator)
            {
                ServiceCallers.Custom.Project(prop => prop.CSATDelete(cSATId, HostingPage.User.Identity.Name));
                tmp.Remove(tmp.First(g => g.Id == cSATId));
            }
            PopulateData(tmp);
            HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT successfully deleted.");
            HostingPage.IsErrorPanelDisplay = true;
            HostingPage.PopulateDirectorsList(ProjectCSATList);
        }

        private void BindScoreDropDown(DropDownList ddlScore)
        {
            ddlScore.Items.Clear();
            ddlScore.Items.Add(new ListItem("-- Select Score --", string.Empty));
            for (int i = 10; i >= 0; i--)
            {
                ddlScore.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

    }
}

