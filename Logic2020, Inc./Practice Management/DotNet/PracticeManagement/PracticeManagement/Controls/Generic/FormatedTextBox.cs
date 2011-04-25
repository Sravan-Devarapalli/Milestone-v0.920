using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic
{
    public class FormatedTextBox : TextBox
    {
        private const string DATA_FORMAT_STRING_VIEWSTATE = "23B828AC-F2F8-470A-B288-B7FFD9EC2EE9";

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

