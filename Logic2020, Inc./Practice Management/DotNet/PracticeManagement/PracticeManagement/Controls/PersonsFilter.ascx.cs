using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls
{
    public partial class PersonsFilter : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Gets if the Active filter was checked.
        /// </summary>
        public bool Active
        {
            get
            {
                return chbShowActive.Checked;
            }
            set
            {
                chbShowActive.Checked = value;
            }
        }


        /// <summary>
        /// Gets if the Projected filter was checked.
        /// </summary>
        public bool Projected
        {
            get
            {
                return chbProjected.Checked;
            }
            set
            {
                chbProjected.Checked = value;
            }
        }


        /// <summary>
        /// Gets if the Terminated filter was checked.
        /// </summary>
        public bool Terminated
        {
            get
            {
                return chbTerminated.Checked;
            }
            set
            {
                chbTerminated.Checked = value;
            }
        }


        /// <summary>
        /// Gets if the Inactive filter was checked.
        /// </summary>
        public bool Inactive
        {
            get
            {
                return chbInactive.Checked;
            }
            set
            {
                chbInactive.Checked = value;
            }
        }


        /// <summary>
        /// Gets the name (?) of the selected practice.
        /// </summary>
        public int? PracticeId
        {
            get
            {
                string stringResult = ddlFilter.SelectedValue;
                return string.IsNullOrEmpty(stringResult) ? (int?)null : Int32.Parse(stringResult);
            }
        }

        /// <summary>
        /// Gets the name (?) of the selected paytype.
        /// </summary>
        public int? PayTypeId
        {
            get
            {
                string stringResult = ddlPayType.SelectedValue;
                return string.IsNullOrEmpty(stringResult) ? (int?)null : Int32.Parse(stringResult);
            }
        }


        /// <summary>
        /// Gets or sets a text about the Active Only checkbox.
        /// </summary>
        public string ActiveOnlyText
        {
            get
            {
                EnsureChildControls();
                return chbShowActive.Text;
            }
            set
            {
                EnsureChildControls();
                chbShowActive.Text = value;
            }
        }

        public event EventHandler FilterChanged;

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            FillPracticeList();
            FillPayTypeList();
        }

        private void FillPracticeList()
        {
            DataHelper.FillPracticeList(ddlFilter, Resources.Controls.AllPracticesText);
        }

        private void FillPayTypeList()
        {
            DataHelper.FillTimescaleList(ddlPayType, Resources.Controls.AllTypes);
        }

        protected void chbShowActive_CheckedChanged(object sender, EventArgs e)
        {
            OnFilterChanged(e);
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilterChanged(e);
        }

        protected void ddlPayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilterChanged(e);
        }

        /// <summary>
        /// Fires the FilterChanged event
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnFilterChanged(EventArgs e)
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, e);
            }
        }
    }
}
