using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace PraticeManagement.Controls
{
    public class CustomDropDown : DropDownList
    {
        public bool IsOptionGroupRequired = true;
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (IsOptionGroupRequired)
            {
                RenderContentWithOptiongroup(writer);
            }
            else
            {
                RenderContentWithOutOptiongroup(writer);
            }
        }
        private void RenderContentWithOutOptiongroup(HtmlTextWriter writer)
        {
            foreach (ListItem item in this.Items)
            {
                RenderListItem(item, writer);
            }
        }
        private void RenderContentWithOptiongroup(HtmlTextWriter writer)
        {
            string currentOptionGroup;
            List<string> renderedOptionGroups = new List<string>();

            foreach (ListItem item in this.Items)
            {
                if (item.Attributes[Constants.Variables.OptionGroup] == null)
                {
                    RenderListItem(item, writer);
                }
                else
                {
                    currentOptionGroup = item.Attributes[Constants.Variables.OptionGroup];

                    if (renderedOptionGroups.Contains(currentOptionGroup))
                    {
                        RenderListItem(item, writer);
                    }
                    else
                    {
                        if (renderedOptionGroups.Count > 0)
                        {
                            RenderOptionGroupEndTag(writer);
                        }

                        RenderOptionGroupBeginTag(currentOptionGroup, writer);
                        renderedOptionGroups.Add(currentOptionGroup);

                        RenderListItem(item, writer);
                    }
                }
            }

            if (renderedOptionGroups.Count > 0)
            {
                RenderOptionGroupEndTag(writer);
            }
        }

        private void RenderOptionGroupBeginTag(string name, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("optgroup");
            writer.WriteAttribute("label", name);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.WriteLine();
        }

        private void RenderOptionGroupEndTag(HtmlTextWriter writer)
        {
            writer.WriteEndTag("optgroup");
            writer.WriteLine();
        }

        private void RenderListItem(ListItem item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("option");
            writer.WriteAttribute("value", item.Value, true);

            if (item.Selected)
            {
                writer.WriteAttribute("selected", "selected", false);
            }

            foreach (string key in item.Attributes.Keys)
            {
                writer.WriteAttribute(key, item.Attributes[key]);
            }

            writer.Write(HtmlTextWriter.TagRightChar);
            HttpUtility.HtmlEncode(item.Text, writer);
            writer.WriteEndTag("option");
            writer.WriteLine();
        }

        protected override object SaveViewState()
        {
            // create object array for Item count + 1 
            object[] allStates = new object[this.Items.Count + 1];
            // the +1 is to hold the base info    
            object baseState = base.SaveViewState();
            allStates[0] = baseState;
            Int32 i = 1;
            // now loop through and save each Style attribute for the List
            foreach (ListItem li in this.Items)
            {
                Int32 j = 0;
                string[][] attributes = new string[li.Attributes.Count][];
                foreach (string attribute in li.Attributes.Keys)
                {
                    attributes[j++] = new string[] { 
                                                        attribute, li.Attributes[attribute] 
                                                   };
                }

                allStates[i++] = attributes;
            }

            return allStates;
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                // restore base first     
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                Int32 i = 1;
                foreach (ListItem li in this.Items)
                {
                    // loop through and restore each style attribute   
                    foreach (string[] attribute in (string[][])myState[i++])
                    {
                        li.Attributes[attribute[0]] = attribute[1];
                    }
                }
            }
        }


    }
}

