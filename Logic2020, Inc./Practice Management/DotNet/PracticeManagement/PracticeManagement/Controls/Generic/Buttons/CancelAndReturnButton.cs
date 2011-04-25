using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public class CancelAndReturnButton : Button
    {
        public CancelAndReturnButton() : base()
        {
            this.Text = Resources.Controls.CancelAndReturn;
            this.Click += new EventHandler(CancelAndReturnButton_Click);
        }

        void CancelAndReturnButton_Click(object sender, EventArgs e)
        {
            PracticeManagementPageBase basePage = this.Page as PracticeManagementPageBase;
            if (basePage != null)
            {
                basePage.ReturnToPreviousPage();
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}
