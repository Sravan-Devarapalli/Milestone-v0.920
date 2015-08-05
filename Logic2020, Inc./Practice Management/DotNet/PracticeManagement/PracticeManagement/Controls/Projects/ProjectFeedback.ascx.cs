using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ProjectService;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectFeedback : System.Web.UI.UserControl
    {

        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public List<ProjectFeedbackStatus> GetStatus
        {
            get
            {
                using (var serviceClient = new ProjectServiceClient())
                {
                    return serviceClient.GetAllFeedbackStatuses().ToList();
                }
            }
        }

        public int FeedbackId
        {
            get
            {
                if (ViewState["FeedbackId"] == null)
                {
                    ViewState["FeedbackId"] = 0;
                }
                return (int)ViewState["FeedbackId"];
            }
            set
            {
                ViewState["FeedbackId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateFeedback();
            }
        }

        public void PopulateFeedback()
        {
            using (var serviceClient = new ProjectServiceClient())
            {
                var feedbacks = serviceClient.GetProjectFeedbackByProjectId(HostingPage.ProjectId.HasValue?HostingPage.ProjectId.Value:-1);
                if (feedbacks.Length > 0)
                {
                    divEmptyMessage.Style["display"] = "none";
                    repFeedback.DataSource = feedbacks;
                    repFeedback.DataBind();
                }
                else
                {
                    divEmptyMessage.Style["display"] = "";
                }
            }
        }

        protected void repFeedback_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem as DataTransferObjects.ProjectFeedback;
                var ddlStatus = e.Item.FindControl("ddlStatus") as DropDownList;
                var lblCompletedCertification = e.Item.FindControl("lblCompletedCertification") as Label;
                var imgCancel = e.Item.FindControl("imgCancel") as ImageButton;
                var lnkCanceled = e.Item.FindControl("lnkCanceled") as LinkButton;
                var lblAsterik = e.Item.FindControl("lblAsterik") as Label;
                
                DataHelper.FillListDefaultWithEncodedName(ddlStatus, string.Empty, GetStatus.ToArray(), true);
                ddlStatus.SelectedValue = dataItem.Status.Id.Value.ToString();
                ddlStatus.Enabled = dataItem.Status.Name != "Canceled";
                imgCancel.Visible = !dataItem.IsCanceled;
                ddlStatus.Visible = true;
                if(dataItem.IsCanceled && dataItem.Status.Name == "Canceled")
                {
                    lblCompletedCertification.Text = String.Format("Project Review canceled by {0} on {1}", dataItem.CompletionCertificateBy, dataItem.CompletionCertificateDate.ToShortDateString());
                    lnkCanceled.Visible = true;
                    ddlStatus.Visible = false;
                }
                else if (dataItem.Status.Name == "Completed")
                {
                    
                    lblCompletedCertification.Text = String.Format("{0} by {1}", dataItem.CompletionCertificateDate.ToShortDateString(), dataItem.CompletionCertificateBy);
                    lnkCanceled.Visible = false;
                    var item = ddlStatus.Items.FindByValue("3"); //canceled status should not be listed for other status records.
                        ddlStatus.Items.Remove(item);
                }
                else
                {
                    lnkCanceled.Visible = false;
                    var item = ddlStatus.Items.FindByValue("3"); //canceled status should not be listed for other status records.
                        ddlStatus.Items.Remove(item);
                }
                lblAsterik.Visible = dataItem.IsGap;
            }
        }

        protected void  ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var statusDropdown = sender as DropDownList;
            var row = statusDropdown.NamingContainer as RepeaterItem;
            var lbl = row.FindControl("lblCompletedCertification") as Label;
            int feedbackId,statusId;
            int.TryParse(lbl.Attributes["FeedbackId"], out feedbackId);
            int.TryParse(statusDropdown.SelectedValue, out statusId);
            FeedbackId = feedbackId;
            using (var service = new ProjectServiceClient())
            {
                service.SaveFeedbackCancelationDetails(FeedbackId, statusId, false, string.Empty, HostingPage.User.Identity.Name,false);
            }
            PopulateFeedback();
        }

        protected void imgCancel_Click(object source, EventArgs e)
        {
            var imgbutton = source as ImageButton;
            modalEx.Show();
            HostingPage.IsOtherPanelDisplay = true;
            btnOk.Text = "Ok";
            tbNotes.Text = "";
            tbNotes.Enabled = true;
            int feedbackId;
            int.TryParse(imgbutton.Attributes["FeedbackId"], out feedbackId);
            FeedbackId = feedbackId;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            var source = sender as Button;
            if (source.Text == "Ok")
            {
                Page.Validate("Feedback");
                if (Page.IsValid)
                {
                    using (var service = new ProjectServiceClient())
                    {
                        service.SaveFeedbackCancelationDetails(FeedbackId, null, true, tbNotes.Text, HostingPage.User.Identity.Name,false);
                    }
                    PopulateFeedback();
                }
            }
            else
            {
                using (var service = new ProjectServiceClient())
                {
                    service.SaveFeedbackCancelationDetails(FeedbackId, 2, false, string.Empty, HostingPage.User.Identity.Name,true);
                }
                PopulateFeedback();
            }
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            var lnkbutton = sender as LinkButton;
            modalEx.Show();
            HostingPage.IsOtherPanelDisplay = true;
            btnOk.Text = "Reactivate";
            tbNotes.Text = lnkbutton.Attributes["FeedbackReason"];
            tbNotes.Enabled = false;
            int feedbackId;
            int.TryParse(lnkbutton.Attributes["FeedbackId"], out feedbackId);
            FeedbackId = feedbackId;
        }
    }
}

