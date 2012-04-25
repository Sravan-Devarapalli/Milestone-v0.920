using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace PraticeManagement.Controls
{
    public partial class FilteredCheckBoxList : System.Web.UI.UserControl
    {
        #region Fields

        private const string OKButtonIdKey = "OKButtonIdKey";
       
        #endregion


        public string OKButtonId
        {
            get
            {
                return ViewState[OKButtonIdKey] as string;
            }
            set
            {
                ViewState[OKButtonIdKey] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnOk.Attributes["onclick"] = string.Format("return btnOk_Click({0});", OKButtonId);
            btnCancel.Attributes["onclick"] = string.Format("return btnCancelButton_Click(\'{0}\');", FilterPopupClientID);
            txtSearchBox.Attributes["onkeyup"] = string.Format("filterTableRows(\'{0}\',\'{1}\',\'{2}\');", txtSearchBox.ClientID, cbl.ClientID, "true");

            AddAttributesToCheckBoxList();
        }

        private void AddAttributesToCheckBoxList()
        {
            foreach (ListItem item in cbl.Items)
            {
                item.Attributes["title"] = item.Text;
            }
        }


        public string SelectedItemsXmlFormat
        {

            get
            {

                if (cbl == null)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<Names>");
                foreach (ListItem item in cbl.Items)
                {
                    if (item.Selected)
                        sb.Append("<Name>" + item.Text + "</Name>");
                }
                sb.Append("</Names>");
                return sb.ToString();
            }
        }


        public Unit Height
        {

            set
            {
                cbl.Height = value;
            }

        }


        public string SelectedIndexes
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < cbl.Items.Count; i++)
                {
                    if (cbl.Items[i].Selected)
                    {
                        sb.Append(i);
                        sb.Append('_');
                    }
                }


                return sb.ToString();
            }
        }

        public void SelectAllItems(bool selectAll)
        {
            foreach (ListItem item in cbl.Items)
            {
                item.Selected = selectAll;
            }
        }

        public ListItemCollection Items { get { return cbl.Items; } }


        public ScrollingDropDown CheckBoxListObject
        {
            get
            {
                return cbl;
            }
        }


        public string FilterPopupClientID
        {
            get
            {
                return divFilterPopUp.ClientID;
            }
        }

        public string WaterMarkTextBoxBehaviorID
        {
            get
            {
                return wmSearch.BehaviorID;
            }
        }


        public string SelectedItems { get { return cbl.SelectedItems; } }
    }
}
