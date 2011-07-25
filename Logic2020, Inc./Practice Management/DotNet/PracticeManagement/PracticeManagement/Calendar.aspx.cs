using System;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.CalendarService;
using PraticeManagement.Controls;
using System.Web.UI.WebControls;

namespace PraticeManagement
{
    public partial class Calendar : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var btnRetrieveCalendar = (Button)calendar.FindControl("btnRetrieveCalendar");
                AsyncPostBackTrigger tr =
                    new AsyncPostBackTrigger() { ControlID = btnRetrieveCalendar.UniqueID, EventName = "Click" };
                pnlHeader.Triggers.Add(tr);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var ddlPerson = (DropDownList)calendar.FindControl("ddlPerson");
            if (ddlPerson != null)
            {
                lblCalendarOwnerName.Text = ddlPerson.SelectedItem.Text;
            }
        }

    }
}

