using System;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public class SingleOption : DropDownList
    {
        #region Constants

        private const string VIEWSTATE_FIRST_ITEM_TEXT = "F160A5BC-29FE-4BC2-8192-5BB82D47D37A";
        private const string VIEWSTATE_NEED_FIRST_ITEM_TEXT = "0CA6DBE6-C597-4C15-A96A-07D7BCA06224";

        #endregion

        #region Properties

        public int? SelectedId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SelectedValue);
                }
                catch
                {
                    return null;
                }
            }
        }

        public string FirstItemText
        {
            get
            {
                var s = (String) ViewState[VIEWSTATE_FIRST_ITEM_TEXT];
                return (s ?? Resources.Controls.SelectItemDefault);
            }

            set { ViewState[VIEWSTATE_FIRST_ITEM_TEXT] = value; }
        }

        public bool NeedFirstItem
        {
            get
            {
                var need = (bool?) ViewState[VIEWSTATE_NEED_FIRST_ITEM_TEXT];
                return need.HasValue ? need.Value : true;
            }

            set { ViewState[VIEWSTATE_NEED_FIRST_ITEM_TEXT] = value; }
        }

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack && NeedFirstItem)
                InsertFirstItem();
        }

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            InsertFirstItem();
        }

        private void InsertFirstItem()
        {
            Items.Insert(0, new ListItem(FirstItemText, string.Empty));
        }

        #endregion
    }
}
