using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;

namespace PraticeManagement
{
    public partial class ProjectCSATDetails : PracticeManagementPageBase
    {

        private bool IsErrorPanelDisplay;

        public int? ProjectId
        {
            get
            {
                return GetArgumentInt32("ProjectId");
            }
        }

        public int? CSATId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    this.hdCSATId.Value = SelectedId.Value.ToString();
                    return SelectedId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.hdCSATId.Value))
                    {
                        int cSATId;
                        if (Int32.TryParse(this.hdCSATId.Value, out cSATId))
                        {
                            return cSATId;
                        }
                    }
                    return null;
                }
            }
            set
            {
                this.hdCSATId.Value = value.ToString();
            }
        }

        public Project Project
        {
            get
            {
                if (ProjectId.HasValue && ViewState["Project_key"] == null)
                {
                    ViewState["Project_key"] = ServiceCallers.Custom.Project(p => p.ProjectGetById(ProjectId.Value));
                }
                return (Project)ViewState["Project_key"];
            }
        }

        public ProjectCSAT CSAT
        {
            get
            {
                if (ProjectId.HasValue)
                {
                    if (ViewState["CSAT_key"] == null)
                    {
                        if (!CSATId.HasValue)
                        {
                            ViewState["CSAT_key"] = new ProjectCSAT();
                        }
                        else
                        {
                            ViewState["CSAT_key"] = ServiceCallers.Custom.Project(p => p.CSATList(ProjectId)).FirstOrDefault(p => p.Id == CSATId.Value);
                        }
                    }
                    return (ProjectCSAT)ViewState["CSAT_key"];
                }
                return null;
            }
            set
            {
                ViewState["CSAT_key"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsErrorPanelDisplay)
            {
                PopulateErrorPanel();
            }
        }

        protected override void Display()
        {
            PopulateData();
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }

        protected void custCSATEndDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (Project.Status.StatusType == ProjectStatusType.Completed)
            {
                DateTime lastCompletedDate = ServiceCallers.Custom.Project(p => p.GetProjectLastChangeDateFortheGivenStatus(Project.Id.Value, Project.Status.Id));
                if (dpReviewEndDate.DateValue.Date > lastCompletedDate.Date)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void custCSATCompleteDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = dpCompletionDate.DateValue.Date <= DateTime.Today.Date ? true : false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumCSAT.ValidationGroup);
            if (Page.IsValid)
            {
                DataTransferObjects.ProjectCSAT pCSAT = new DataTransferObjects.ProjectCSAT();
                pCSAT.ProjectId = ProjectId.Value;
                pCSAT.ReferralScore = int.Parse(ddlScore.SelectedValue);
                pCSAT.ReviewStartDate = dpReviewStartDate.DateValue;
                pCSAT.ReviewEndDate = dpReviewEndDate.DateValue;
                pCSAT.CompletionDate = dpCompletionDate.DateValue;
                pCSAT.ReviewerId = int.Parse(ddlReviewer.SelectedValue);
                pCSAT.Comments = taComments.Value;
                if (!CSATId.HasValue)
                {
                  CSATId=  ServiceCallers.Custom.Project(p => p.CSATInsert(pCSAT, DataHelper.CurrentPerson.Alias));
                    mlConfirmation.ShowInfoMessage("CSAT successfully added.");
                }
                else
                {
                    pCSAT.Id = CSATId.Value;
                    ServiceCallers.Custom.Project(p => p.CSATUpdate(pCSAT, DataHelper.CurrentPerson.Alias));
                    mlConfirmation.ShowInfoMessage("CSAT successfully updated.");
                }
                ReturnToPreviousPage();
            }
            IsErrorPanelDisplay = true;
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

        public void PopulateData()
        {
            BindScoreDropDown(ddlScore);
            List<int> excludedPerson = new List<int>();
            if (Project.Director != null)
            {
                excludedPerson.Add(Project.Director.Id.Value);
            }
            DataHelper.FillCSATReviewerList(ddlReviewer, "-- Select Reviewer --", excludedPerson);
            if (CSATId.HasValue)
            {
                ddlScore.SelectedValue = CSAT.ReferralScore.ToString();
                dpReviewStartDate.DateValue = CSAT.ReviewStartDate;
                dpReviewEndDate.DateValue = CSAT.ReviewEndDate;
                dpCompletionDate.DateValue = CSAT.CompletionDate;
                ddlReviewer.SelectedValue = CSAT.ReviewerId.ToString();
                taComments.Value = CSAT.Comments;
            }
            else
            {
                ddlScore.SelectedValue = string.Empty;
                dpReviewStartDate.DateValue = Project.StartDate.HasValue ? Project.StartDate.Value : DateTime.Now.Date;
                dpReviewEndDate.DateValue = Project.EndDate.HasValue ? Project.EndDate.Value : DateTime.Now.Date;
                dpCompletionDate.DateValue = DateTime.Now.Date;
                ddlReviewer.SelectedIndex = 0;
                taComments.Value = "";
            }
            lblProjectName.Text = Project.Name;
        }

    }
}
