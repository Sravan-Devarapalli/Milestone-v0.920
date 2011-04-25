using System;
using System.Web.UI;
using PraticeManagement.Events;

namespace PraticeManagement.Controls
{
    public class PracticeManagementUserControl : UserControl
    {
        #region Protected Methods

        protected new PracticeManagementPageBase Page
        {
            get { return base.Page as PracticeManagementPageBase; }
        }

        protected T GetViewStateValue<T>(string key, T defValue)
        {
            var value = ViewState[key];

            return value == null ? defValue : (T) value;
        }

        protected void SetViewStateValue<T>(string key, T value){
            ViewState[key] = value;    
        }

        #endregion

        #region

        public EventHandler<ErrorEventArgs> CustomError;

        #endregion
    }
}

