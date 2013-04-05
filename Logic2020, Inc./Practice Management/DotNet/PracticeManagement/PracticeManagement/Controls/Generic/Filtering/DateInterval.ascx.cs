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

