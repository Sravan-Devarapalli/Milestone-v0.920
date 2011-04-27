using System;
using System.Web.UI;
using PraticeManagement.Controls;
namespace PraticeManagement.Config
{
    public partial class ConsultantUtilTimelineReport : PracticeManagementPageBase, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void Display()
        {
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            utf.SaveFilters();
            Redirect(eventArgument);
        }
    }
}
