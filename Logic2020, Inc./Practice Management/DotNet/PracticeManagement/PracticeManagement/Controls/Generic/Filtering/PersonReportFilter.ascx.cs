using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using System.Web.Security;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class PersonReportFilter : System.Web.UI.UserControl
    {
        public List<int> SelectedValues
        {
            get { return this.cblPersons.SelectedValues;}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentPerson = DataHelper.CurrentPerson;
                var personRoles = Roles.GetRolesForUser(currentPerson.Alias);
                string statusIdsList = GetStatusIds();
                int? personId = null;
                if(!personRoles.Any(s => s=="Administrator"))
                {
                    personId = currentPerson.Id;
                }
                DataHelper.FillTimeEntryPersonList(this.cblPersons, Resources.Controls.AllPersons, null, statusIdsList, personId);
            }

            cbInActive.InputAttributes.Add("style", "float: left !Important;width:20px;margin-right:0px;");
            cbProjected.InputAttributes.Add("style", "float: left !Important;width:20px;margin-right:0px;");
            cbTerminated.InputAttributes.Add("style", "float: left !Important;width:20px;margin-right:0px;");

        }

        //protected void odsPersons_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        //{
        //    e.InputParameters["PersonStatusIdsList"] = GetStatusIds();
        //}

        private string GetStatusIds()
        {
            string statusList = ((int)PersonStatusType.Active).ToString();
            if (this.cbInActive.Checked)
                statusList += "," + ((int)PersonStatusType.Inactive).ToString();
            if (this.cbProjected.Checked)
                statusList += "," + ((int)PersonStatusType.Projected).ToString();
            if (this.cbTerminated.Checked)
                statusList += "," + ((int)PersonStatusType.Terminated).ToString();

            return statusList;
        }

        protected void status_OnCheckedChanged(object sendeer, EventArgs e)
        {
            var currentPerson = DataHelper.CurrentPerson;
            var personRoles = Roles.GetRolesForUser(currentPerson.Alias);
            string statusIdsList = GetStatusIds();
            int? personId = null;
            if (!personRoles.Any(s => s == "Administrator"))
            {
                personId = currentPerson.Id;
            }
            DataHelper.FillTimeEntryPersonList(this.cblPersons, Resources.Controls.AllPersons, null, statusIdsList, personId);
        }
    }
}
