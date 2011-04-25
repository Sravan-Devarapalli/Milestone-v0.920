using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls
{
    public partial class ProjectNameCellRounded : System.Web.UI.UserControl
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
        /// Main Button CssClass Name
        /// </summary>
        public string ButtonCssClass
        {
            set
            {
                btnProjectName.CssClass = value;
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


        public int ToolTipOffsetX
        {
            set
            {
                hm.OffsetX = value;
            }
        }

        public int ToolTipOffsetY
        {
            set
            {
                hm.OffsetY = value;
            }
        }

        public AjaxControlToolkit.HoverMenuPopupPosition ToolTipPopupPosition
        {
            set
            {
                hm.PopupPosition = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
