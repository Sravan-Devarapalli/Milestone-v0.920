﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.GuestPages
{
    public partial class ChangePasswordError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnContinue_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(Constants.ApplicationPages.LoginPage);
        }
    }
}
