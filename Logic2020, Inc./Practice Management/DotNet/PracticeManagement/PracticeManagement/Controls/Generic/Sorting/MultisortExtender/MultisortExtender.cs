﻿using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.Sorting.MultisortExtender
{
    [ClientScriptResource(
        "PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior",
        "PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortExtender.js")]
    [TargetControlType(typeof(Label))]
    public class MultisortExtender : ExtenderControlBase
    {
        #region Constants

        private const string SYNCHRONIZER_ID = "SynchronizerID";
        private const string ASCENDING_TEXT = "AscendingText";
        private const string DESCENDING_TEXT = "DescendingText";
        private const string NO_SORTING_TEXT = "NoSortingText";
        private const string SORT_EXPRESSION = "SortExpression";
        private const string ORDER_BY_CLAUSE = "ORDER BY ";

        private static char ITEMS_SEPARATOR = ',';
        private static char DIRECTION_SEPARATOR = ':';
        private static char ORDER_BY_SEPARATOR = ',';

        #endregion        
        
        #region Properties

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("SynchronizerIDValue")]
        public string SynchronizerID
        {
            get
            {
                return GetPropertyValue(SYNCHRONIZER_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(SYNCHRONIZER_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("SortExpressionValue")]
        public string SortExpression
        {
            get
            {
                return GetPropertyValue(SORT_EXPRESSION, string.Empty);
            }
            set
            {
                SetPropertyValue(SORT_EXPRESSION, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("&uarr;")]
        [ClientPropertyName("AscendingTextValue")]
        public string AscendingText
        {
            get
            {
                return GetPropertyValue(ASCENDING_TEXT, string.Empty);
            }
            set
            {
                SetPropertyValue(ASCENDING_TEXT, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("&darr;")]
        [ClientPropertyName("DescendingTextValue")]
        public string DescendingText
        {
            get
            {
                return GetPropertyValue(DESCENDING_TEXT, string.Empty);
            }
            set
            {
                SetPropertyValue(DESCENDING_TEXT, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("x")]
        [ClientPropertyName("NoSortingTextValue")]
        public string NoSortingText
        {
            get
            {
                return GetPropertyValue(NO_SORTING_TEXT, string.Empty);
            }
            set
            {
                SetPropertyValue(NO_SORTING_TEXT, value);
            }
        } 

        #endregion

        #region Utils

        /// <summary>
        /// Returns sort expression from synchronization string
        /// </summary>
        /// <param name="syncString">Sync string generated by extenders</param>
        /// <returns>Sort expression ready to be passed to SQL server</returns>
        public static string GetSortString(string syncString)
        {
            StringBuilder orderBy = new StringBuilder();

            string[] sortExpressions = syncString.Split(new char[] { ITEMS_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string expr in sortExpressions)
            {
                string[] orderItem = expr.Split(DIRECTION_SEPARATOR);
                orderBy.Append(orderItem[0]).Append(" ").Append(orderItem[1]).Append(ORDER_BY_SEPARATOR);
            }

            string sortExpression = orderBy.ToString();
            return sortExpression.Length == 0 ? 
                string.Empty : 
                ORDER_BY_CLAUSE + sortExpression.TrimEnd(ORDER_BY_SEPARATOR);
        }

        #endregion
    }
}

