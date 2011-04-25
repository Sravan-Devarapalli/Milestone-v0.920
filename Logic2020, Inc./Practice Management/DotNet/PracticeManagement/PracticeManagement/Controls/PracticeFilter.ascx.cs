using System;


namespace PraticeManagement.Controls
{
	public partial class PracticeFilter : System.Web.UI.UserControl
	{
		#region Properties

		/// <summary>
		/// Gets if the Active Only filter was checked.
		/// </summary>
		public bool ActiveOnly
		{
			get
			{
				return chbShowActiveOnly.Checked;
			}
            set
            {
                chbShowActiveOnly.Checked = value;
            }
		}

		/// <summary>
		/// Gets the name (?) of the selected practice.
		/// </summary>
		public int? PracticeId
		{
			get
			{
				string stringResult = ddlFilter.SelectedValue;
				return string.IsNullOrEmpty(stringResult) ? (int?)null : Int32.Parse(stringResult);
			}
		}

		/// <summary>
		/// Gets or sets a text about the Active Only checkbox.
		/// </summary>
		public string ActiveOnlyText
		{
			get
			{
				EnsureChildControls();
				return chbShowActiveOnly.Text;
			}
			set
			{
				EnsureChildControls();
				chbShowActiveOnly.Text = value;
			}
		}

		public event EventHandler FilterChanged;

		#endregion

		protected void Page_Init(object sender, EventArgs e)
		{
			FillPracticeList();
		}

		private void FillPracticeList()
		{
			DataHelper.FillPracticeList(ddlFilter, Resources.Controls.AllPracticesText);
		}

		protected void chbShowActiveOnly_CheckedChanged(object sender, EventArgs e)
		{
			OnFilterChanged(e);
		}

		protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnFilterChanged(e);
		}

		/// <summary>
		/// Fires the FilterChanged event
		/// </summary>
		/// <param name="e">The event arguments.</param>
		private void OnFilterChanged(EventArgs e)
		{
			if (FilterChanged != null)
			{
				FilterChanged(this, e);
			}
		}
	}
}
