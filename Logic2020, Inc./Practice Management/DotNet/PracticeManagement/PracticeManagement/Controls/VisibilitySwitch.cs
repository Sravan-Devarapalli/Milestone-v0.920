using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls
{
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:VisibilitySwitch runat=server></{0}:VisibilitySwitch>")]
	public class VisibilitySwitch : WebControl, IPostBackEventHandler
	{
		#region Constants

		private const string TextKey = "Text";
		private const string ControlIdKey = "ControlId";
		private const string RenderScriptFormat = "javascript:{0}";
		private const string EmtyTextOutputFormat = "[{0}]";

		#endregion

		#region Fields

		private string textValue;
		private string controlIdValue;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a text to be displayed within the control.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Text
		{
			get
			{
				if (EnableViewState)
				{
					textValue = (string)ViewState[TextKey];
				}
				return textValue ?? string.Empty;
			}

			set
			{
				textValue = value;
				if (EnableViewState)
				{
					ViewState[TextKey] = textValue;
				}
			}
		}

		/// <summary>
		/// Gets or sets an ID of the handled control.
		/// </summary>
		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("")]
		[Localizable(false)]
		public string ControlID
		{
			get
			{
				if (EnableViewState)
				{
					controlIdValue = (string)ViewState[ControlIdKey];
				}
				return  controlIdValue ?? string.Empty;
			}
			set
			{
				controlIdValue = value;
				if (EnableViewState)
				{
					ViewState[ControlIdKey] = controlIdValue;
				}
			}
		}

		#endregion

		#region Construction

		public VisibilitySwitch()
			: base(HtmlTextWriterTag.A)
		{
		}

		#endregion

		#region IPostBackEventHandler Members

		/// <summary>
		/// Switches the visibility of the specified control.
		/// </summary>
		/// <param name="eventArgument">Dummy</param>
		public void RaisePostBackEvent(string eventArgument)
		{
			Control ctrl = Parent.FindControl(ControlID);
			if (ctrl == null)
			{
				throw new ControlNotFoundException(ControlID);
			}
			ctrl.Visible = !ctrl.Visible;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the attributes to the output writer.
		/// </summary>
		/// <param name="writer">The writer to the attrubutes are added to.</param>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);

			string postBackReference =
				Page.ClientScript.GetPostBackEventReference(this, null, false);
			writer.AddAttribute(HtmlTextWriterAttribute.Href,
				string.Format(RenderScriptFormat, postBackReference));
		}

		/// <summary>
		/// Renders an HTML markup.
		/// </summary>
		/// <param name="output">The output writer to the control be rendered to.</param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			if (DesignMode && string.IsNullOrEmpty(Text))
			{
				output.Write(string.Format(EmtyTextOutputFormat, ID));
			}
			else
			{
				output.Write(Text);
			}
		}

		#endregion
	}
}

