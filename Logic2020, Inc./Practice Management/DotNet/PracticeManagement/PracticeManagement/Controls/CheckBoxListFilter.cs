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
            function imgSort_Click(button)
            {
                alert(button.getAttribute('sort'));
                var cblList = document.getElementById(cbl).getElementsByTagName('input');
                for (var i = 0; i < indexesArray.length; i++)
			      {
                   if(indexesArray[i] != '')
			       cblList[indexesArray[i]].checked = true;
			      }
            }
            function Filter_Click(filterdiv, selectedIndexes, cbl) {
                filterdiv.style.display = '';
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

        private const string submitButtonsScript = @"<table class='WholeWidth'><tr>
                                                        <td align='right' style='padding:3px;'>
                                                           <input onclick='btnOk_Click({0});' type='button' value='OK' title='OK' style='width:60px;' />    
                                                           <input runat='server' onclick='btnCancel_Click({1});'  type='button' value='Cancel' title='Cancel' style='width:60px;' />
                                                        </td>
                                                    </tr></table>";

        private const string sortButtonsScript = @"<table class='WholeWidth'>
                                                        <tr><td align='right' style='padding:3px;'>
                                                           <img onclick='imgSort_Click(this,{0});' sort='asc' title='Ascending' style='width:100%;height:20px;' src='../Images/add_16.png'/>    
                                                        </td></tr>
                                                        <tr><td align='right' style='padding:3px;'>
                                                           <img onclick='imgSort_Click(this,{1});' sort='desc' title='Descending' style='width:100%;height:20px;' src='../Images/add_16.png' />    
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
            writer.WriteLine(string.Format("<div id='{0}' style='border:2px solid black;background-color:white;'>", FilterPopupId));
           // writer.WriteLine(string.Format(sortButtonsScript,this.ClientID,this.ClientID));
            base.RenderControl(writer);
            writer.WriteLine(string.Format(submitButtonsScript, OKButtonId, FilterPopupId));
            writer.WriteLine("</div>");
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

