using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class Option : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InsertFirstItem();
        }

        private void InsertFirstItem()
        {
            ddlOptions.Items.Insert(0, new ListItem(FistItemText, string.Empty));
        }

        public string FistItemText { get;  set; }

        public object DataSource
        {
            get
            {
                return ddlOptions.DataSource;
            }

            set
            {
                ddlOptions.DataSource = value;
            }
        }

        public void DataBindControl()
        {
            ddlOptions.DataBind();
            InsertFirstItem();
        }

    //    public string DataTextField
    //    {
    //    get{
    //    return ddlo
    //}
    //}

        public int? SelectedId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ddlOptions.SelectedValue);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
