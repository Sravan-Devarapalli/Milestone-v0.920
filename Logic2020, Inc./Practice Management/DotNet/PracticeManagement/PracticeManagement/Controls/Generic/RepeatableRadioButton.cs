using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace PraticeManagement.Controls.Generic
{
    public class RepeatableRadioButton : System.Web.UI.WebControls.RadioButton, IPostBackDataHandler
    {
        protected string ParentUniqueId
        {
            get
            {
                return this.Parent.Parent.UniqueID;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new RepeatableRadioButtonWriter(writer,
            ParentUniqueId, GroupName));
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey,
        System.Collections.Specialized.NameValueCollection postCollection)
        {
            bool isChecked = this.Checked;
            if (postCollection[String.Format("{0}:{1}", ParentUniqueId, GroupName)] != null)
            {
                this.Checked = postCollection[String.Format("{0}:{1}", ParentUniqueId, GroupName)].Equals(this.UniqueID);
            }
            return isChecked ^ this.Checked;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            base.OnCheckedChanged(EventArgs.Empty);
        }
    }

    public class RepeatableRadioButtonWriter : HtmlTextWriter
    {
        string _parentUniqueId = null;
        string _groupName = null;

        public RepeatableRadioButtonWriter(HtmlTextWriter writer, string
        parentUniqueId, string groupName)
            : base(writer)
        {
            _parentUniqueId = parentUniqueId;
            _groupName = groupName;
        }

        public override void AddAttribute(HtmlTextWriterAttribute key,
        string value)
        {
            if (key == HtmlTextWriterAttribute.Name)
                value = String.Format("{0}:{1}", _parentUniqueId, _groupName);
            base.AddAttribute(key, value);
        }
    }
}
