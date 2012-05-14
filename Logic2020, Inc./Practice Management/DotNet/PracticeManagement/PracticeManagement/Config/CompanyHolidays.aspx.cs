using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement.Config
{
    public partial class CompanyHolidays : PracticeManagementPageBase
    {
        public PraticeManagement.Controls.Calendar CalendarControl
        {
            get
            {
                return calendar;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void Display()
        {
        }
    }
}
