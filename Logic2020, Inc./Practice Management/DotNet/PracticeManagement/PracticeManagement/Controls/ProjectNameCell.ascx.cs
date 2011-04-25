using System;

namespace PraticeManagement.Controls
{
    public partial class ProjectNameCell : System.Web.UI.UserControl
    {
        #region Property
        
        /// <summary>
        /// Project Name Button Id
        /// </summary>
        public string ButtonProjectNameId
        {            
            set
            {
                btnProjectName.ID = value;
            }
        }

        /// <summary>
        /// Project Name Button Text
        /// </summary>
        public string ButtonProjectNameText
        {            
            set
            {
                btnProjectName.Text = value;
            }
        }
        
        /// <summary>
        /// Project Name Button ToolTip content
        /// </summary>
        public string ButtonProjectNameToolTip
        {            
            set
            {
                lblTooltipContent.Text = value;
            }
        }

        /// <summary>
        /// Project Name Button ToolTip content
        /// </summary>
        public string ButtonProjectNameHref
        {
            set
            {
                btnProjectName.NavigateUrl = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
