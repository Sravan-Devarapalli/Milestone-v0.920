using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;

namespace PraticeManagement
{
    public partial class Financial : PracticeManagementPageBase, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["ActiveTabIndex"]))
            {
                int activeTabIndex = 0;
                if (int.TryParse(Request.QueryString["ActiveTabIndex"], out activeTabIndex))
                {
                    tabSettings.ActiveTabIndex = activeTabIndex;
                }
            }
        }
        protected override void Display()
        {
        }

        public bool PageBaseSaveDirty
        {
            get
            {
                return SaveDirty;
            }
        }

        public void PageBaseClearDirty()
        {
            ClearDirty();
        }
        
        public void RaisePostBackEvent(string eventArgument)
        {
            if (SaveDirty)
            {
                if (ucBudget.ValidateAndSaveRevenueGoals())
                {
                    if (eventArgument.Contains("Financial.aspx#?"))
                    {
                        string element = eventArgument.ElementAt(eventArgument.Length - 1).ToString();
                        tabSettings.ActiveTabIndex = int.Parse(element);
                    }
                    else
                    {
                        Redirect(eventArgument);
                    }
                }
                else
                {
                    tabSettings.ActiveTabIndex = 4;
                }
            }
            else
            {
                if (eventArgument.Contains("Financial.aspx#?"))
                {
                    string element = eventArgument.ElementAt(eventArgument.Length - 1).ToString();
                    tabSettings.ActiveTabIndex = int.Parse(element);
                }
                if (IsDirty)
                {
                    ucBudget.LoadRevenueGoals();
                    PageBaseClearDirty();
                }
            }
        }
    }
}
