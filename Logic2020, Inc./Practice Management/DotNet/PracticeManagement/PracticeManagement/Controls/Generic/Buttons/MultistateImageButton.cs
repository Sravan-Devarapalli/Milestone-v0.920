using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public interface IMultistateImageButton
    {
        string EntityName { get; }
        int EntityId { get; set; }
    }

    public abstract class MultistateImageButton<T>: ImageButton, IMultistateImageButton
    {
        #region Constatnts

        private const string VsEntityId = "VS_EntityId";
        private const string VsState = "VS_State";

        #endregion

        #region Protected stuff

        protected abstract Dictionary<T, KeyValuePair<string, string>> FilenameMapping { get; }

        #endregion

        #region Properties

        public T State
        {
            get
            {
                return (T) ViewState[VsState];
            }

            set { ViewState[VsState] = value; }
        }

        #endregion

        #region IMultistateImageButton members

        public abstract string EntityName { get; }

        public int EntityId
        {
            get { return (int) ViewState[VsEntityId]; }

            set { ViewState[VsEntityId] = value; }
        }


        #endregion

        #region Overrides

        public override string ImageUrl
        {
            get
            {
                return FilenameMapping[State].Key;
            }
        }

        public override string ToolTip
        {
            get
            {
                return FilenameMapping[State].Value;
            }
        }

        public override string  AlternateText
        {
            get
            {
                return FilenameMapping[State].Value;
            }
        }

        protected override string Text
        {
            get
            {
                return FilenameMapping[State].Value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("span");
            writer.WriteAttribute("title", ToolTip);
            //  The WriteBeginTag method does not write the closing angle bracket (>) 
            //  Use the TagRightChar  constant to close the opening tag.
            writer.Write(HtmlTextWriter.TagRightChar);

            base.Render(writer);

            writer.WriteEndTag("span");
        }

        #endregion

    }
}



