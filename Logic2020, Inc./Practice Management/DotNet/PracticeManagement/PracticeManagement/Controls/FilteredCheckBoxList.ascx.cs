﻿using System;
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
                return ViewState["SelectedInsexes" + this.ClientID] as string;
            }
        }


        public List<int> SelectedIndexesList
        {
           
            get
            {
                List<int> result = new List<int>();
                var list = SelectedIndexes.Split('_');

                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        result.Add(Convert.ToInt32(item));
                    }
                }

                return result;
            }
        }


        public string ActualSelectedItems
        {
            get
            {
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


        public string ActualSelectedItemsXmlFormat
        {
            get
            {
                if (SelectedIndexesList.Count == 0)
                    return "";

                // Check if All checkbox is checked
                if (SelectedIndexesList.Any(s => s == 0))
                    return null;

                // If not, build comma separated list of values
                var sb = new StringBuilder();
                sb.Append("<Names>");
                for (int i = 0; i < SelectedIndexesList.Count; i++)
                {
                    sb.Append("<Name>" + cbl.Items[SelectedIndexesList[i]].Text + "</Name>");
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

            ViewState["SelectedInsexes" + this.ClientID] = sb.ToString();
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

