using System;
using System.Drawing;

namespace PraticeManagement.Controls
{
    /// <summary>
    /// Message types control can display
    /// </summary>
    public enum MessageType{
        /// <summary>
        /// Error
        /// </summary>
        Error,

        /// <summary>
        /// Warning
        /// </summary>
        Warning,

        /// <summary>
        /// Info
        /// </summary>
        Info,

        /// <summary>
        /// Deafult is error
        /// </summary>
        Default = Error
    }

    public partial class MessageLabel : System.Web.UI.UserControl
    {
        #region Properties

        public Color ErrorColor { get; set; }
        public Color InfoColor { get; set; }
        public Color WarningColor { get; set; }
        public string CssClass { set { lblMessage.CssClass = value; } }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Show methods

        public void ClearMessage()
        {
            lblMessage.Text = string.Empty;
            lblMessage.Visible = false;
        }

        protected void ShowMessage(MessageType mType, string format, string message)
        {
            ShowMessage(mType, string.Format(format, message));
        }

        protected void ShowMessage(MessageType mType, string message)
        {
            lblMessage.Visible = true;
            lblMessage.Text = message;

            switch (mType)
            {
                case MessageType.Error:
                    lblMessage.ForeColor = ErrorColor;
                    break;

                case MessageType.Info:
                    lblMessage.ForeColor = InfoColor;
                    break;

                case MessageType.Warning:
                    lblMessage.ForeColor = WarningColor;
                    break;
            }
        }

        public void ShowErrorMessage(string format, string message)
        {
            ShowMessage(MessageType.Error, format, message);
        }

        public void ShowInfoMessage(string format, string message)
        {
            ShowMessage(MessageType.Info, format, message);
        }

        public void ShowWarningMessage(string format, string message)
        {
            ShowMessage(MessageType.Warning, format, message);
        }

        public void ShowErrorMessage(string message)
        {
            ShowMessage(MessageType.Error, message);
        }

        public void ShowInfoMessage(string message)
        {
            ShowMessage(MessageType.Info, message);
        }

        public void ShowWarningMessage(string message)
        {
            ShowMessage(MessageType.Warning, message);
        }

        #endregion
    }
}
