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
        public PraticeManagement.Controls.PersonCalendar CalendarControl
        {
            get
            {
                return calendar;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        { 
        }

        

    }
}

