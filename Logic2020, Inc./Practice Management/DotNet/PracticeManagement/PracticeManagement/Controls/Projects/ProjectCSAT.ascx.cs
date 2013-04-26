using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

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
        }

        public void PopulateData(List<DataTransferObjects.ProjectCSAT> projectCSATList)
        {
            ProjectCSATList = projectCSATList;
            gvCSAT.DataSource = ProjectCSATList;
            gvCSAT.DataBind();
        }

        public void PopulatePopUp(DataTransferObjects.ProjectCSAT csat)
        {
            BindScoreDropDown(ddlScore);
            List<int> excludedPerson = new List<int>();
            DataHelper.FillCASTReviewerList(ddlReviewer, "-- Select Reviewer--", excludedPerson);
            if (csat != null)
            {
                ddlScore.SelectedValue = csat.ReferralScore.ToString();
                dpReviewStartDate.DateValue = csat.ReviewStartDate;
                dpReviewEndDate.DateValue = csat.ReviewEndDate;
                dpCompletionDate.DateValue = csat.CompletionDate;
                ddlReviewer.SelectedValue = csat.ReviewerId.ToString();
                taComments.Value = csat.Comments;
            }
            else
            {
                ddlScore.SelectedValue = "0";
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
                DataTransferObjects.ProjectCSAT pCSAT = new DataTransferObjects.ProjectCSAT ();
                pCSAT.ReferralScore = int.Parse(ddlScore.SelectedValue);
                pCSAT.ReviewStartDate = dpReviewStartDate.DateValue;
                pCSAT.ReviewEndDate = dpReviewEndDate.DateValue;
                pCSAT.CompletionDate = dpCompletionDate.DateValue;
                pCSAT.ReviewerId = int.Parse(ddlReviewer.SelectedValue);
                pCSAT.Comments = taComments.Value;
                ServiceCallers.Custom.Project(p => p.CSATInsert(pCSAT, DataHelper.CurrentPerson.Alias));
                PopulateData(null);
            }
        }

        protected void stbCSAT_Click(object sender, EventArgs e)
        {
            if (HostingPage.ProjectId.HasValue)
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
                    if (HostingPage.Project.Director != null)
                    {
                        excludedPerson.Add(HostingPage.Project.Director.Id.Value);
                    }
                    DataHelper.FillCASTReviewerList(ddlReviewer, "-- Select Client Director --", excludedPerson);
                    ddlReviewer.SelectedValue = dataItem.ReviewerId.ToString();
                }
                else
                {
                    ImageButton imgDeleteCSAT = (ImageButton)e.Row.FindControl("imgDeleteCSAT");
                    imgDeleteCSAT.Visible = userIsAdministrator;
                }
            }
        }

        protected void btnReviewStartDate_Command(object sender, CommandEventArgs e)
        {

        }

        protected void imgCopyCSAT_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCopy = sender as ImageButton;
            var row = imgCopy.NamingContainer as GridViewRow;
            int cSATId = int.Parse(((HiddenField)row.FindControl("hdCSATId")).Value);
            ProjectCSATList = null;
            var copyCSAT = ProjectCSATList.First(s => s.Id == cSATId);
            hdnCopyCSAT.Value = copyCSAT.Id.ToString();
            ProjectCSATList.Add(new DataTransferObjects.ProjectCSAT()
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
            gvCSAT.EditIndex = ProjectCSATList.Count - 1;
            PopulateData(ProjectCSATList);
        }

        protected void imgEditCSAT_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvCSATItem = imgEdit.NamingContainer as GridViewRow;
            gvCSAT.EditIndex = gvCSATItem.DataItemIndex;
            PopulateData(ProjectCSATList);
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
                }
                else
                {
                    ServiceCallers.Custom.Project(p => p.CSATCopyFromExistingCSAT(pCSAT, int.Parse(hdnCopyCSAT.Value), DataHelper.CurrentPerson.Alias));
                }
                gvCSAT.EditIndex = -1;
                PopulateData(tmp);
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
        }

        private void BindScoreDropDown(DropDownList ddlScore)
        {
            ddlScore.Items.Clear();
            ddlScore.Items.Add(new ListItem(0.ToString(), "--Select Score--"));
            for (int i = 10; i > 0; i--)
            {
                ddlScore.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

    }
}
