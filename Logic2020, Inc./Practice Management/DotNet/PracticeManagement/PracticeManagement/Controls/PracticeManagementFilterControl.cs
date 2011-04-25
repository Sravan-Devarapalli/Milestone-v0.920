using System;
using System.Web.UI;
using PraticeManagement.Events;
using System.Web.UI.WebControls;
using System.Web;

namespace PraticeManagement.Controls
{
    public abstract class PracticeManagementFilterControl<T> : PracticeManagementUserControl where T : class, new()
    {
        private static T _filter;

        public event EventHandler FilterOptionsChanged;

        public static T Filter
        {
            get
            {
                T sessionFilter = null;
                if (typeof(T).ToString() == "DataTransferObjects.ContextObjects.OpportunityListContext")
                {
                    sessionFilter = HttpContext.Current.Session["opListContext"] as T;
                }
                else
                {
                    sessionFilter = HttpContext.Current.Session["opSortingContext"] as T;
                }

                if (sessionFilter == null)
                    sessionFilter = SerializationHelper.DeserializeCookie<T>(typeof(T).ToString());

                return sessionFilter ?? new T();
            }
            set
            {
                if (typeof(T).ToString() == "DataTransferObjects.ContextObjects.OpportunityListContext")
                {
                    HttpContext.Current.Session["opListContext"] = value;
                }
                else
                {
                    HttpContext.Current.Session["opSortingContext"] = value;
                }

                T sessionFilter = value;
                SerializationHelper.SerializeCookie(sessionFilter, typeof(T).ToString());
            }
        }

        protected abstract void ResetControls();
        protected abstract void InitControls();
        protected abstract T InitFilter();

        protected void UpdateFilter()
        {
            Filter = InitFilter();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                InitControls();
        }

        public void ResetFilter()
        {
            ResetControls();
            Filter = new T();
        }

        protected void FireFilterOptionsChanged()
        {
            UpdateFilter();
            Utils.Generic.InvokeEventHandler(FilterOptionsChanged, this);
        }

        protected static int? GetDropDownValueWithDefault(DropDownList ddl)
        {
            return ddl.SelectedValue == string.Empty ? (int?)null : Convert.ToInt32(ddl.SelectedValue);
        }
    }
}

