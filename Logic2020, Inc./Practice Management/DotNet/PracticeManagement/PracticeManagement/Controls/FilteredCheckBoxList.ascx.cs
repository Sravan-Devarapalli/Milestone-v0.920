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

        public bool AllItemsSelected
        {
            get
            {
                return CheckBoxListObject.Items.Count == SelectedIndexesList.Count || CheckBoxListObject.Items.Count - 1 == SelectedIndexesList.Count;
            }
        }

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
            btnOk.Attributes["onclick"] = string.Format("return btnOk_Click(\'{0}\',\'{1}\',\'{2}\');", OKButtonId, hdnSelectedIndexes.ClientID, cbl.ClientID);
            btnCancel.Attributes["onclick"] = string.Format("return btnCancelButton_Click(\'{0}\');", FilterPopupClientID);
            txtSearchBox.Attributes["onkeyup"] = string.Format("filterTableRows(\'{0}\',\'{1}\',\'{2}\');", txtSearchBox.ClientID, cbl.ClientID, "true");

            AddAttributesToCheckBoxList();
        }

        private void AddAttributesToCheckBoxList()
        {
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Value == string.Empty && i != 0)
                {
                    cbl.Items[i].Attributes["style"] = "font-style: italic;";
                }

                cbl.Items[i].Attributes["title"] = HttpUtility.HtmlDecode(cbl.Items[i].Text);
            }


        }

        public Unit Height
        {

            set
            {
                cbl.Height = value;
            }

        }

        public string CssClass
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    cbl.CssClass = "FilteredCheckBoxListScrollingDropDown " + value;
                }
            }
        }

        public string SelectedIndexes
        {
            get
            {
                return hdnSelectedIndexes.Value;
            }
        }

        public List<int> SelectedIndexesList
        {

            get
            {
                List<int> result = new List<int>();

                if (!string.IsNullOrEmpty(SelectedIndexes))
                {
                    var list = SelectedIndexes.Split('_');

                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            result.Add(Convert.ToInt32(item));
                        }
                    }
                }

                return result;
            }
        }

        public string SelectedItems
        {
            get
            {
                SetSelectedIndexesForCBL();

                if (SelectedIndexesList.Count == 0)
                    return "";

                // Check if All checkbox is checked
                if (SelectedIndexesList.Any(s => s == 0))
                    return null;

                // If not, build comma separated list of values
                var sb = new StringBuilder();

                for (int i = 0; i < SelectedIndexesList.Count; i++)
                {
                    sb.Append(cbl.Items[SelectedIndexesList[i]].Value).Append(',');
                }

                return sb.ToString();
            }
        }

        public string SelectedItemsXmlFormat
        {
            get
            {
                SetSelectedIndexesForCBL();

                if (SelectedIndexesList.Count == 0)
                    return "";

                // Check if All checkbox is checked
                if (SelectedIndexesList.Any(s => s == 0) || SelectedIndexesList.Max() >= cbl.Items.Count)
                    return null;

                // If not, build comma separated list of values
                var sb = new StringBuilder();
                sb.Append("<Names>");
                for (int i = 0; i < SelectedIndexesList.Count; i++)
                {
                    sb.Append("<Name>" + cbl.Items[SelectedIndexesList[i]].Value + "</Name>");
                }
                sb.Append("</Names>");

                return sb.ToString();
            }

        }

        public void SaveSelectedIndexesInViewState()
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

            hdnSelectedIndexes.Value = sb.ToString();
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

        private void SetSelectedIndexesForCBL()
        {
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                cbl.Items[i].Selected = false;

                if (SelectedIndexesList.Any(indexVal => i == indexVal))
                {
                    cbl.Items[i].Selected = true;
                }
            }
        }
    }
}

