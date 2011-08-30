using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class RealInterval : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public double MinValue
        {
            get
            {
                return ParseDouble(tbFrom, 0.0);
            }
            set
            {
                tbFrom.Text = value.ToString();
            }
        }

        public double MaxValue
        {
            get
            {
                return ParseDouble(tbTo, 24.0);
            }
            set
            {
                tbTo.Text = value.ToString();
            }
        }

        private static double ParseDouble(TextBox sourceTextBox, double defaultValue)
        {
            try
            {
                return Convert.ToDouble(sourceTextBox.Text);
            }
            catch
            {

                return defaultValue;
            }
        }

        public void Reset()
        {
            tbTo.Text = "24.0";
            tbFrom.Text = "0.0";
        }
    }
}

