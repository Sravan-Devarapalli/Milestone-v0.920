using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.HtmlControls;
using DataTransferObjects;


namespace PraticeManagement.Controls
{
    public class MultipleSelectionCheckBoxList : CheckBoxList
    {

        #region Constants

        /// <summary>
        /// Script key to insure script was included only once
        /// </summary>
        private const string GeneralScriptKey = "MultipleSelectionCheckBoxListScriptKey";

        private const string GeneralScriptSource =
            @"
         <script type=""text/javascript"" src=""../Scripts/jquery-1.4.1.js""></script>
         <script type=""text/javascript"">

            function currCheckbox_Onclick(cb)
            {
               var strikecheckboxid = cb.parentNode.getAttribute('strikecheckboxid');
                var strikeCheckbox =document.getElementById(strikecheckboxid);
                if(cb.checked)
                {
                    strikeCheckbox.checked = !cb.checked;
                    strikeCheckbox.disabled ='disabled';
                }
                else
                {
                    strikeCheckbox.disabled ='';
                }
            }

            function strikeCB_Onclick(sCB)
            {
                var checkboxid = sCB.parentNode.getAttribute('checkboxid');
                var checkBox = document.getElementById(checkboxid);
                if(sCB.checked)
                {
                   checkBox.checked = !sCB.checked;
                   checkBox.disabled ='disabled';
                }
                else
                {
                    checkBox.disabled ='';
                }
            }

            function MultipleSelectionCheckBoxes_OnClick(ControlClientID) {
                changeAlternateitemsForProposedResources(ControlClientID);
                var trPotentialResources = document.getElementById(ControlClientID).getElementsByTagName('tr');
                for (var i = 0; i < trPotentialResources.length; i++) {
                    var currCheckbox = trPotentialResources[i].children[0].getElementsByTagName('input')[0];
                    var strikeCheckbox = trPotentialResources[i].children[1].getElementsByTagName('input')[0];
                    var func = 'currCheckbox_Onclick('+currCheckbox.id.toString()+');';
                    currCheckbox.setAttribute('onclick', func);
                    var funcName = 'strikeCB_Onclick('+strikeCheckbox.id.toString()+');';
                    strikeCheckbox.setAttribute('onclick', funcName);
                }
            }
             function changeAlternateitemsForProposedResources(ControlClientID) {
                    var trPotentialResources = document.getElementById(ControlClientID).getElementsByTagName('tr');
                    var index = 0;
                    for (var i = 0; i < trPotentialResources.length; i++) {
                        if (trPotentialResources[i].style.display != 'none') {
                            index++;
                            if ((index) % 2 == 0) {
                                trPotentialResources[i].style.backgroundColor = '#f9faff';
                            }
                            else {
                                trPotentialResources[i].style.backgroundColor = '';
                            }
                        }
                    }
                }
        </script>
        ";

        #endregion

        protected override void OnInit(EventArgs e)
        {
            //  Include general script and ensure that it was included only once
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GeneralScriptKey))
                Page.ClientScript.RegisterClientScriptBlock(
                    Page.GetType(), GeneralScriptKey, GeneralScriptSource, false);
            base.OnInit(e);
        }

        protected override void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {
            ListItem item = this.Items[repeatIndex];

            item.Attributes["strikecheckboxid"] = "strikeCheckBox" + repeatIndex.ToString();
            item.Attributes["persontype"] = ((int)OpportunityPersonType.NormalPerson).ToString();
            item.Attributes["personid"] = item.Value;
            item.Attributes["personname"] = item.Text;

            CheckBox cb = new CheckBox();
            cb.ID = "strikeCheckBox" + repeatIndex.ToString();
            cb.Attributes["checkboxid"] = this.ClientID + "_" + repeatIndex.ToString();
            cb.Attributes["personid"] = item.Value;
            cb.Attributes["personname"] = item.Text;
            cb.Attributes["persontype"] = ((int)OpportunityPersonType.StrikedPerson).ToString();


            if (item.Attributes["selectedchecktype"] != null)
            {
                int typeId = Convert.ToInt32(item.Attributes["selectedchecktype"]);

                if (typeId == (int)OpportunityPersonType.NormalPerson)
                {
                    item.Selected = true;
                    cb.Checked = false;
                }
                else
                {
                    item.Selected = false;
                    cb.Checked = true;
                }

            }

            cb.Enabled = item.Enabled;
            base.RenderItem(itemType, repeatIndex, repeatInfo, writer);
            writer.Write("</td><td style='padding:0px;width:10px;' align='left' valign='top'>");
            cb.RenderControl(writer);

        }

    }
}
