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
            btnSavePopUp.Visible =
            btnCancelPopUp.Visible =
            HostingPage.CSATTabEditPermission && (HostingPage.SelectedStatus == (int)ProjectStatusType.Active || (HostingPage.SelectedStatus == (int)ProjectStatusType.Completed && HostingPage.Project.Milestones.Count > 0));
        }

        public void PopulateData(List<DataTransferObjects.ProjectCSAT> projectCSATList)
        {
            ProjectCSATList = projectCSATList;
            gvCSAT.DataSource = ProjectCSATList;
            gvCSAT.DataBind();
        }

        protected void custCSATCompletionDateInGridView_ServerValidate(object sender,ServerValidateEventArgs e)
        {
	    e.IsValid = true;
            CustomValidator custCSATCompletionDate = (CustomValidator)sender;
            GridViewRow row = custCSATCompletionDate.NamingContainer as GridViewRow;
            DatePicker dpCompletionDate = (DatePicker)row.FindControl("dpCompletionDate");
            if (HostingPage.Project.Status.StatusType == ProjectStatusType.Completed)
            {
                DateTime lastCompletedDate = ServiceCallers.Custom.Project(p => p.GetProjectLastChangeDateFortheGivenStatus(HostingPage.Project.Id.Value, HostingPage.Project.Status.Id));
                e.IsValid = dpCompletionDate.DateValue.Date <= lastCompletedDate.Date;
            }
        }

        protected void custCSATCompletionDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (HostingPage.Project.Status.StatusType == ProjectStatusType.Completed)
            {
                DateTime lastCompletedDate = ServiceCallers.Custom.Project(p => p.GetProjectLastChangeDateFortheGivenStatus(HostingPage.Project.Id.Value, HostingPage.Project.Status.Id));
               if (dpCompletionDate.DateValue.Date > lastCompletedDate.Date)
               {
                   e.IsValid = false;
               }
               else
               {
                   e.IsValid = true;
               }
            }
            else
            {
                e.IsValid = true;
            }
        }

        public void PopulatePopUp(DataTransferObjects.ProjectCSAT csat)
        {
            BindScoreDropDown(ddlScore);
            List<int> excludedPerson = new List<int>();
            if (HostingPage.Project.CSATOwnerId != -1)
            {
                excludedPerson.Add(HostingPage.Project.CSATOwnerId);
            }
            if (HostingPage.Project.Director.Id != null)
            {
                excludedPerson.Add(HostingPage.Project.Director.Id.Value);
            }
            DataHelper.FillCSATReviewerList(ddlReviewer, "-- Select Reviewer--", excludedPerson);
            if (csat != null)
            {
                ddlScore.SelectedValue = csat.ReferralScore.ToString();
                dpReviewStartDate.DateValue = csat.ReviewStartDate;
                dpReviewEndDate.DateValue = csat.ReviewEndDate;
                dpCompletionDate.DateValue = csat.CompletionDate;
                ddlReviewer.SelectedValue = csat.ReviewerId.ToString();
                taComments.Value = csat.Comments;
                hdnSelectedCSATId.Value = csat.Id.ToString();
            }
            else
            {
                ddlScore.SelectedValue = "";
                dpReviewStartDate.DateValue = HostingPage.Project.StartDate.Value;
                dpReviewEndDate.DateValue = HostingPage.Project.EndDate.Value;
                dpCompletionDate.DateValue = DateTime.Now.Date;
                ddlReviewer.SelectedIndex = 0;
                taComments.Value = "";
            }
        }

        protected void btnCancelPopUp_Click(object sender, EventArgs e)
        {
            mpeCSAT.Hide();
        }

        protected void btnSavePopUp_Click(object sender, EventArgs e)
        {
            Page.Validate("CSATPopup");
            if (Page.IsValid)
            {
                DataTransferObjects.ProjectCSAT pCSAT = new DataTransferObjects.ProjectCSAT();
                pCSAT.ProjectId = HostingPage.ProjectId.Value;
                pCSAT.ReferralScore = int.Parse(ddlScore.SelectedValue);
                pCSAT.ReviewStartDate = dpReviewStartDate.DateValue;
                pCSAT.ReviewEndDate = dpReviewEndDate.DateValue;
                pCSAT.CompletionDate = dpCompletionDate.DateValue;
                pCSAT.ReviewerId = int.Parse(ddlReviewer.SelectedValue);
                pCSAT.Comments = taComments.Value;
                if (hdnPopupAddOrUpdate.Value == "Add")
                {
                    ServiceCallers.Custom.Project(p => p.CSATInsert(pCSAT, DataHelper.CurrentPerson.Alias));
                    HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT Successfully Added.");
                }
                else if (hdnPopupAddOrUpdate.Value == "Update")
                {
                    pCSAT.Id = Convert.ToInt32(hdnSelectedCSATId.Value);
                    ServiceCallers.Custom.Project(p => p.CSATUpdate(pCSAT, DataHelper.CurrentPerson.Alias));
                    HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT Successfully Updated.");
                    hdnPopupAddOrUpdate.Value = "Add";
                }
                PopulateData(null);
              
                HostingPage.IsErrorPanelDisplay = true;
            }
            else
            {
                mpeCSAT.Show();
            }
        }

        protected void stbCSAT_Click(object sender, EventArgs e)
        {
            if (HostingPage.ProjectId.HasValue && HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                PopulatePopUp(null);
                mpeCSAT.Show();
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
                    if (dataItem.ReviewerId != HostingPage.Project.CSATOwnerId)
                    {
                        excludedPerson.Add(HostingPage.Project.CSATOwnerId);
                    }
                    
                    DataHelper.FillCSATReviewerList(ddlReviewer, "-- Select Client Director --", excludedPerson);
                    ddlReviewer.SelectedValue = dataItem.ReviewerId.ToString();
                }
                else
                {
                    ImageButton imgDeleteCSAT = (ImageButton)e.Row.FindControl("imgDeleteCSAT");
                    imgDeleteCSAT.Visible = userIsAdministrator;
                    ImageButton imgCopyCSAT = (ImageButton)e.Row.FindControl("imgCopyCSAT");
                    ImageButton imgEditCSAT = (ImageButton)e.Row.FindControl("imgEditCSAT");
                    imgCopyCSAT.Visible = imgEditCSAT.Visible = HostingPage.CSATTabEditPermission && (HostingPage.SelectedStatus == (int)ProjectStatusType.Active || HostingPage.SelectedStatus == (int)ProjectStatusType.Completed);

                }
            }
        }

        protected void btnReviewStartDate_Command(object sender, CommandEventArgs e)
        {
            if (HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                var selectedCSAT = ProjectCSATList.First(p => p.Id == Convert.ToInt32(e.CommandArgument));
                hdnPopupAddOrUpdate.Value = "Update";
                PopulatePopUp(selectedCSAT);
                mpeCSAT.Show();
            }
        }

        protected void imgCopyCSAT_OnClick(object sender, EventArgs e)
        {
            if (HostingPage.ValidateAndSaveFromOtherChildControls())
            {
                ImageButton imgCopy = sender as ImageButton;
                var row = imgCopy.NamingContainer as GridViewRow;
                int cSATId = int.Parse(((HiddenField)row.FindControl("hdCSATId")).Value);
                ProjectCSATList = null;
                var copyCSAT = ProjectCSATList.First(s => s.Id == cSATId);
                hdnCopyCSAT.Value = copyCSAT.Id.ToString();
                var tmp = ProjectCSATList;
                tmp.Add(new DataTransferObjects.ProjectCSAT()
                {
                    Id = -1,
                    ProjectId = copyCSAT.ProjectId,
                    ReferralScore = copyCSAT.ReferralScore,
                    ReviewStartDate = copyCSAT.ReviewStartDate,
                    ReviewEndDate = copyCSAT.ReviewEndDate,
                    CompletionDate = copyCSAT.CompletionDate,
                    Comments = copyCSAT.Comments,
                    ReviewerId = copyCSAT.ReviewerId
                });
                ProjectCSATList = tmp;
                gvCSAT.EditIndex = ProjectCSATList.Count - 1;
                PopulateData(ProjectCSATList);
            }
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
            hdnCopyCSAT.Value = "-1";
            ProjectCSATList = null;
            PopulateData(ProjectCSATList);
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
                if (hdnCopyCSAT.Value == "-1")
                {
                    ServiceCallers.Custom.Project(p => p.CSATUpdate(pCSAT, DataHelper.CurrentPerson.Alias));
                    HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT Successfully Updated.");
                    ProjectCSATList = null;
                    tmp = ProjectCSATList;
                }
                else
                {
                    ServiceCallers.Custom.Project(p => p.CSATCopyFromExistingCSAT(pCSAT, int.Parse(hdnCopyCSAT.Value), DataHelper.CurrentPerson.Alias));
                    HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT Successfully Added.");
                    ProjectCSATList = null;
                    tmp = ProjectCSATList;
                    hdnCopyCSAT.Value = "-1";
                }
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
            HostingPage.mlConfirmationControl.ShowInfoMessage("CSAT Successfully Deleted.");
            HostingPage.IsErrorPanelDisplay = true;
        }

        private void BindScoreDropDown(DropDownList ddlScore)
        {
            ddlScore.Items.Clear();
            ddlScore.Items.Add(new ListItem("--Select Score--", string.Empty));
            for (int i = 10; i > 0; i--)
            {
                ddlScore.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

    }
}

