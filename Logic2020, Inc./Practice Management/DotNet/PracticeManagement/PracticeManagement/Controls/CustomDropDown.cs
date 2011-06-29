﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace PraticeManagement.Controls
{
    public class CustomDropDown : DropDownList
    {
        /*
            if (Items != null)
            {
                bool selected = false;

                foreach (ListItem listItem in Items)
                {
                    writer.WriteBeginTag("option");

                    if (listItem.Selected)
                    {
                        if (selected)
                            //throw new HttpException(HttpRuntime.FormatResourceString("Cannot_Multiselect_In_DropDownList"));
                            throw new HttpException("Cannot multiselect in DropDownList."); //TODO: this should be language independent
                        selected = true;
                        writer.WriteAttribute("selected", "selected", false);
                    }
                    writer.WriteAttribute("value", listItem.Value, true);

                    listItem.Attributes.Render(writer); // This is the added code
                    //TODO: What happens if "value" or "selected" are in listItem.Attributes?

                    writer.Write('>');
                    HttpUtility.HtmlEncode(listItem.Text, writer);

                    writer.WriteEndTag("option");
                    writer.WriteLine();
                }
            }*/

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
