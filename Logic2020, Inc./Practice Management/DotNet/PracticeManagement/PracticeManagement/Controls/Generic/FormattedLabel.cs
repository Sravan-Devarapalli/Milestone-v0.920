using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic
{
    public class FormattedLabel : Label
    {
        private const string DATA_FORMAT_STRING_VIEWSTATE = "FormattedLabelViewState";

        public DateTime DateText
        {
            get
            {
                return Utils.Generic.ParseDate(base.Text);
            }
            set
            {
                base.Text = value.ToString(DataFormatString);
            }
        }

        public double DoubleText
        {
            get
            {
                return Convert.ToDouble(base.Text);
            }

            set
            {
                base.Text = value.ToString(Constants.Formatting.DoubleFormat);
            }
        }

        public string DataFormatString
        {
            get
            {
                string dfs = ViewState[DATA_FORMAT_STRING_VIEWSTATE] as string;
                return dfs ?? string.Empty;
            }

            set
            {
                ViewState[DATA_FORMAT_STRING_VIEWSTATE] = value;
            }
        }
    }
}
