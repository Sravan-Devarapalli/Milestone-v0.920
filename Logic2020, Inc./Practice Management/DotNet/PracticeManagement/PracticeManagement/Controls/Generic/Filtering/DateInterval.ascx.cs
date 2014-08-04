using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class DateInterval1 : System.Web.UI.UserControl
    {
        public int? FromToDateFieldWidth { get; set; }
        public string FromToDateFieldCssClass { get; set; }
        public bool IsFromDateRequired { get; set; }
        public bool IsToDateRequired { get; set; }
        public bool IsBillingReport { get; set; }

        public string OnClientChange
        {
            set
            {
                this.clFromDate.OnClientDateSelectionChanged = this.clToDate.OnClientDateSelectionChanged = value;
            }
        }

        public string ValidationGroup
        {
            get { return tbFrom.ValidationGroup; }
            set
            {
                tbFrom.ValidationGroup =
                reqValFrom.ValidationGroup =
                valFrom.ValidationGroup =
                rangeValFrom.ValidationGroup =
                tbTo.ValidationGroup =
                reqValTo.ValidationGroup =
                valTo.ValidationGroup =
                rangeValTo.ValidationGroup =
                compToDate.ValidationGroup = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(this.IsBillingReport)
            {
                lblFrom.Text = "Start date";
                lblTo.Text = "End date";
                reqValFrom.ToolTip = reqValFrom.ErrorMessage = "Start date is required.";
                reqValTo.ToolTip = reqValTo.ErrorMessage = "End date is required";
                rangeValFrom.ToolTip = rangeValFrom.ErrorMessage = "Start date should be between 1/1/1985 and 12/31/2100";
                rangeValTo.ToolTip = rangeValTo.ErrorMessage = "End date date should be between 1/1/1985 and 12/31/2100";
                compToDate.ToolTip = compToDate.ErrorMessage = "End date must be greater or equal to the Start date.";
            }
            else
            {
                lblFrom.Text = "from";
                lblTo.Text = "to";
                reqValFrom.ToolTip = reqValFrom.ErrorMessage = "From date is required.";
                reqValTo.ToolTip = reqValTo.ErrorMessage = "To date is required";
                rangeValFrom.ToolTip = rangeValFrom.ErrorMessage = "From date should be between 1/1/1985 and 12/31/2100";
                rangeValTo.ToolTip = rangeValTo.ErrorMessage = "To date should be between 1/1/1985 and 12/31/2100";
                compToDate.ToolTip = compToDate.ErrorMessage = "To date must be greater or equal to the from date.";
            }
            if (this.FromToDateFieldWidth.HasValue)
            {
                this.tbFrom.Width = this.FromToDateFieldWidth.Value;
                this.tbTo.Width = this.FromToDateFieldWidth.Value;
            }

            if (!string.IsNullOrEmpty(FromToDateFieldCssClass))
            {
                this.tbFrom.CssClass = this.FromToDateFieldCssClass + " MarginClass";
                this.tbTo.CssClass = this.FromToDateFieldCssClass + " MarginClass";
            }
            else
            {
                this.tbFrom.CssClass = "MarginClass width50Px";
                this.tbTo.CssClass = "MarginClass width50Px";
            }

            reqValFrom.Enabled = IsFromDateRequired;
            reqValTo.Enabled = IsToDateRequired;
        }

        public bool Enabled
        {
            get
            {
                return tbFrom.Enabled;
            }
            set
            {
                tbFrom.Enabled = value;
                tbTo.Enabled = value;
                clFromDate.Enabled = value;
                clToDate.Enabled = value;
            }
        }

        public DateTime? FromDate
        {
            get
            {
                return ParseDateTime(tbFrom);
            }
            set
            {
                tbFrom.Text = value.HasValue ? value.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
            }
        }

        public DateTime? ToDate
        {
            get
            {
                return ParseDateTime(tbTo);
            }
            set
            {
                tbTo.Text = value.HasValue ? value.Value.ToString(Constants.Formatting.EntryDateFormat) : string.Empty;
            }
        }

        private static DateTime? ParseDateTime(TextBox tb)
        {
            string dateText = tb.Text;

            if (string.IsNullOrEmpty(dateText))
                return null;

            try
            {
                return DateTime.Parse(dateText);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public void Reset()
        {
            tbTo.Text = string.Empty;
            tbFrom.Text = string.Empty;
        }


    }
}

