using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PraticeManagement.PersonStatusService;
using System.ServiceModel;

namespace PraticeManagement.Controls.Reports
{
    public partial class BadgeResourcesByPracticeFilter : System.Web.UI.UserControl
    {
        public string Practices
        {
            get
            {
                var clientList = new StringBuilder();
                foreach (ListItem item in cblPractices.Items)
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');
                return clientList.ToString();
            }
        }

        public string PayTypes
        {
            get
            {
                return cblPayTypes.areAllSelected ? null : cblPayTypes.SelectedItems;
            }
        }

        public string PersonStatus
        {
            get
            {
                var clientList = new StringBuilder();
                foreach (ListItem item in cblPersonStatus.Items)
                {
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');
                    if (item.Value == "1" && item.Selected)
                    {
                        clientList.Append("2").Append(',');
                        clientList.Append("5").Append(',');
                    }
                }
                return clientList.ToString();
            }
        }

        public string TitleIds
        {
            get
            {
                var clientList = new StringBuilder();
                foreach (ListItem item in cblTitles.Items)
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');
                return clientList.ToString();
            }
        }

        public int ddlType //1-practice area, 2-business title, 3-technical title
        {
            get;
            set;
        }

        public bool IsBadgedNotOnProject
        {
            get
            {
                return chbBadgedNotOnProject.Checked;
            }
        }

        public bool IsBadgedOnProject
        {
            get
            {
                return chbBadgedOnProject.Checked;
            }
        }

        public bool IsClockNotStarted
        {
            get
            {
                return chbClockNotStarted.Checked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ddlType == 1)
                {
                    DataHelper.FillPracticeListOnlyActive(cblPractices, Resources.Controls.AllPracticesText);
                    cblPractices.SelectAll();
                    tdPractices.Visible = true;
                    tdTitles.Visible = false;
                    lblCategory.Text = "Practice Area";
                }
                else
                {
                    var titles = ServiceCallers.Custom.Title(t => t.GetAllTitles());
                    titles = titles.Where(t => t.TitleType.TitleTypeId == ddlType - 1).ToArray();
                    DataHelper.FillTitleList(cblTitles, "All Titles",true,ddlType-1);
                    cblTitles.SelectAll();
                    tdTitles.Visible = true;
                    tdPractices.Visible = false;
                    lblCategory.Text = "Title";
                }

                DataHelper.FillTimescaleList(this.cblPayTypes, Resources.Controls.AllTypes);
                cblPayTypes.SelectItems(new List<int>() { 1, 2 });
                FillPersonStatusList();
                cblPersonStatus.SelectItems(new List<int>() { 1, 5 });
            }
        }

        public void FillPersonStatusList()
        {
            using (var serviceClient = new PersonStatusServiceClient())
            {
                try
                {
                    var statuses = serviceClient.GetPersonStatuses();
                    statuses = statuses.Where(p => p.Id != 2 && p.Id != 5).ToArray();
                    DataHelper.FillListDefault(cblPersonStatus, Resources.Controls.AllTypes, statuses, false);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }
    }
}

