using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls
{
    [ValidationProperty("TextValue")]
    public partial class DatePicker : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// Text version of the control's value, for use by ASP.NET validators.
        /// </summary>
        [Browsable(false)]
        public string TextValue
        {
            get
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                dateRangeVal.Enabled = DateValue > DateTime.MinValue;
                return txtDate.Text;
            }
        }

        /// <summary>
        /// Gets or sets whether the Auto Post backs are enabled.
        /// </summary>
        [Category("Behaviour")]
        [DefaultValue(false)]
        [Localizable(false)]
        public bool AutoPostBack
        {
            get
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                return txtDate.AutoPostBack;
            }
            set
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                txtDate.AutoPostBack = value;
            }
        }

        /// <summary>
        /// Holds the current date value of this control.
        /// </summary>
        [Category("Behaviour")]
        [Localizable(true)]
        [Bindable(true, BindingDirection.TwoWay)]
        public DateTime DateValue
        {
            get
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                try
                {
                    if (txtDate.Text == "") return DateTime.MinValue;

                    DateTime val;
                    return DateTime.TryParse(txtDate.Text, out val) ? val : DateTime.MinValue;
                }
                catch (ArgumentNullException)
                {
                    return DateTime.MinValue;
                }
                catch (FormatException)
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                if (value == DateTime.MinValue)
                {
                    txtDate.Text = string.Empty;
                }
                else
                {
                    txtDate.Text = value.ToShortDateString();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the control is read-only.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Localizable(false)]
        public bool ReadOnly
        {
            get
            {
                EnsureChildControls();
                return txtDate.ReadOnly;
            }
            set
            {
                EnsureChildControls();
                txtDate.ReadOnly = value;
                lnkCalendar.Visible = !value;
                txtDate_CalendarExtender.Enabled = !value;
            }
        }

        /// <summary>
        /// Gets or sets a width for the text field.
        /// </summary>
        [Category("Appearance")]
        public Unit TextBoxWidth
        {
            get
            {
                EnsureChildControls();
                return txtDate.Width;
            }
            set
            {
                EnsureChildControls();
                txtDate.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets a validation group for an internal validator.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        public string ValidationGroup
        {
            get
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                return dateRangeVal.ValidationGroup;
            }
            set
            {
                if (!DesignMode)
                {
                    EnsureChildControls();
                }
                dateRangeVal.ValidationGroup = value;
            }
        }

        public string OnClientChange
        {
            set;
            get;
        }

        public string ErrorMessage 
        { 
            get { return dateRangeVal.ErrorMessage; }
            set { dateRangeVal.ErrorMessage = value; } 
        }

        #endregion

        #region Events

        /// <summary>
        /// A day was selected from the calendar control.
        /// </summary>
        public event EventHandler SelectionChanged;

        #endregion

        #region Methods

        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)   // only raise the event if someone is listening.
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            dateRangeVal.MinimumValue = new DateTime(2001, 1, 1).ToShortDateString();
            dateRangeVal.MaximumValue = new DateTime(2029, 12, 31).ToShortDateString();
            dateRangeVal.ErrorMessage = dateRangeVal.ToolTip =
                string.Format(Resources.Messages.DateRangeMessage,
                dateRangeVal.MinimumValue,
                dateRangeVal.MaximumValue);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(OnClientChange))
            {
                txtDate.Attributes.Add("onchange", OnClientChange);
            }
        }

        #endregion
    }
}
