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
        private const string GeneralScriptKey = "CheckBoxListFilterScriptKey";
        private const string FilterButtonIdKey = "FilterButtonIdKey";


        #endregion


        //<cc2:CheckBoxListFilter ID="cblSeniorities" runat="server" BorderColor="#aaaaaa"
        // AllSelectedReturnType="Null" Height="155px" BackColor="White" CellPadding="3"
        // NoItemsType="All" SetDirty="False" Width="160px" BorderWidth="0" />



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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnOk.Attributes["onclick"] = string.Format("return btnOk_Click({0});", OKButtonId);
            btnCancel.Attributes["onclick"] = string.Format("return btnCancelButton_Click(\'{0}\');", FilterPopupId);
            txtSearchBox.Attributes["onkeyup"] = string.Format("filterTableRows(\'{0}\',\'{1}\',\'{2}\');", SearchTextBoxId, cbl.ClientID, "true");
        }

        public bool SetDirty
        {

            set { cbl.SetDirty = value; }
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


        public PraticeManagement.Controls.ScrollingDropDown.AllSelectedType AllSelectedReturnType
        {

            set { cbl.AllSelectedReturnType = value; }
        }

        public Unit Height
        {

            set
            {
                cbl.Height = value;
            }

        }

        public Unit Width
        {
            set
            {
                cbl.Width = value;
            }
        }

        public PraticeManagement.Controls.ScrollingDropDown.NoItemsBehaviour NoItemsType
        {

            set { cbl.NoItemsType = value; }
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


        public string FilterPopupId
        {
            get
            {
                return divFilterPopUp.ClientID;
            }
        }

        public string SearchTextBoxId
        {
            get
            {
                return txtSearchBox.ClientID;
            }
        }

        public string SelectedItems { get { return cbl.SelectedItems; } }
    }
}
