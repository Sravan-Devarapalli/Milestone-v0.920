﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement
{
    public partial class LoggedOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((PracticeManagementMain)Master)._PageTitle = "Logged Out";
            ServiceCallers.Custom.Person(p => p.DeleteReportFilterValues(PracticeManagementMain.CurrentUserID,PracticeManagementMain.PreviousUserId));
        }
    }
}

