using System;
using PraticeManagement.Controls;
using System.Web.UI;

namespace PraticeManagement.Config
{
    public partial class DefaultLineManager : PracticeManagementPageBase, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void Display()
        {
        }

        public new void ClearDirty()
        {
            base.ClearDirty();
        }

        #region RaisePostBackEventHandler
        
        public void RaisePostBackEvent(string eventArgument)
        {
            if (defaultManager.SavedNewDefaultManager())
            {
                Redirect(eventArgument);
            }
        }
        
        #endregion
    }
}
