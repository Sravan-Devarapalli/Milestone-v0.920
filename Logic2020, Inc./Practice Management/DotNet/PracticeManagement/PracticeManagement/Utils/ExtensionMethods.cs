using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections;

namespace PraticeManagement.Utils
{
    public static class ExtensionMethods
    {
        #region "DropDown Sorting Methods"

        public static void SortByText(this ListControl ddList)
        {
            SortDropDown(ddList, new DropDownTextComparer());
        }

        public static void SortByValue(this ListControl ddList)
        {
            SortDropDown(ddList, new DropDownValueComparer());
        }

        private static void SortDropDown(ListControl ddList, IComparer comparer)
        {
            int i;
            if (ddList.Items.Count <= 1)
                return;

            ArrayList arrItems = new ArrayList();
            for (i = 0; i < ddList.Items.Count; i++)
            {
                ListItem item = ddList.Items[i];
                arrItems.Add(item);
            }

            arrItems.Sort(comparer);

            ddList.Items.Clear();

            for (i = 0; i < arrItems.Count; i++)
            {
                ddList.Items.Add((ListItem)arrItems[i]);
            }
        }

        private class DropDownTextComparer : IComparer
        {
            public int Compare(Object o1, Object o2)
            {
                ListItem cb1 = (ListItem)o1;
                ListItem cb2 = (ListItem)o2;
                return cb1.Text.CompareTo(cb2.Text);
            }
        }

        private class DropDownValueComparer : IComparer
        {
            public int Compare(Object o1, Object o2)
            {
                ListItem cb1 = (ListItem)o1;
                ListItem cb2 = (ListItem)o2;
                return cb1.Value.CompareTo(cb2.Value);
            }
        }

        #endregion "DropDown Sorting Methods"
    }
}
