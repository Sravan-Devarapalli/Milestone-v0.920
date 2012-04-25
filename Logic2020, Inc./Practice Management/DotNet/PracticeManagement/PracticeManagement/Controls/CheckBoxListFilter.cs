using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Text;
using System.Xml.Linq;

namespace PraticeManagement.Controls
{
    [ToolboxData("<{0}:CheckBoxListFilter runat=server></{0}:CheckBoxListFilter>")]
    public class CheckBoxListFilter : ScrollingDropDown
    {

        #region Fields

        private const string OKButtonIdKey = "OKButtonIdKey";
        private const string GeneralScriptKey = "CheckBoxListFilterScriptKey";
        private const string FilterButtonIdKey = "FilterButtonIdKey";
        private const string GeneralScriptSource =
          @"
         <script src=""../Scripts/FilterTable.js"" type=""text/javascript""></script>
         <script type=""text/javascript"">

            function btnOk_Click(okButton) {
                okButton.click();
            }

            function btnCancel_Click(filterdiv) {
              filterdiv.style.display = 'none';
            }

            function uncheckAllCheckBoxes(cblList)
            {
             for (var i = 0; i < cblList.length; i++)
			 {
			   cblList[i].checked = false;
			 }
              
            }

            function Filter_Click(filterdiv, selectedIndexes, cbl,txtSearchBox) {
                filterdiv.style.display = '';
                txtSearchBox.value = '';
                filterTableRows(txtSearchBox, cbl, true);
                var indexesArray = selectedIndexes.split('_');
                var cblList = document.getElementById(cbl).getElementsByTagName('input');

                uncheckAllCheckBoxes(cblList);

              for (var i = 0; i < indexesArray.length; i++)
			  {
               if(indexesArray[i] != '')
			   cblList[indexesArray[i]].checked = true;
			  }
            }
        </script>
        ";

        private const string submitButtonsScript = @"<table style='width:{2}'><tr>
                                                        <td align='right' style='padding:6px 0px 6px 0px;'>
                                                           <input onclick='btnOk_Click({0});' type='button' value='OK' title='OK' style='width:60px;' />    
                                                           <input onclick='btnCancel_Click({1});'  type='button' value='Cancel' title='Cancel' style='width:60px;' />
                                                        </td>
                                                    </tr></table>";


        private const string searchScript = @"<table style='width:{3}'>
                                                        <tr><td align='right' style='padding:6px 0px 6px 0px; text-align:left;'>
                                                           <input type='text' id='{0}' onkeyup='filterTableRows({0},{1},{2});'  style='width:100%;text-align:left;border-color:black;' />
                                                        </td></tr>
                                                    </table>";
      
        #endregion

        public string FilterPopupId
        {
            get
            {
                return "div" + ClientID;
            }
        }

        public string SearchTextBoxId
        {
            get
            {
                return (this.ClientID + "_txtSearch");
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

        public string SelectedItemsXmlFormat
        {
            get
            {
                if (SelectedItems == null)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<Names>");
                foreach (ListItem item in this.Items)
                {
                    if (item.Selected)
                        sb.Append("<Name>" + item.Text + "</Name>");
                }
                sb.Append("</Names>");
                return sb.ToString();
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.WriteLine(string.Format("<div id='{0}' style='border:2px solid black;background-color:#d4dff8;width:175px;'>", FilterPopupId));
            writer.WriteLine("<table style='text-align:center;width:100%;' ><tr><td align='center'>");
            writer.WriteLine(string.Format(searchScript, SearchTextBoxId, this.ClientID, "true", Width.IsEmpty ? "150px" : Width.ToString()));
            writer.WriteLine("</td></tr><tr><td style='width:100%'>");
            base.RenderControl(writer);
            writer.WriteLine("</td></tr><tr><td style='width:100%' align='center'>");
            writer.WriteLine(string.Format(submitButtonsScript, OKButtonId, FilterPopupId,Width.IsEmpty ? "150px" : Width.ToString()));
            writer.WriteLine("</table></div>");
        }

        protected override void OnInit(EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GeneralScriptKey))
                Page.ClientScript.RegisterClientScriptBlock(
                    Page.GetType(), GeneralScriptKey, GeneralScriptSource, false);

            base.OnInit(e);
        }

        public string SelectedIndexes
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
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
            foreach (ListItem item in this.Items)
            {
                item.Selected = selectAll;
            }
        }

    }
}

