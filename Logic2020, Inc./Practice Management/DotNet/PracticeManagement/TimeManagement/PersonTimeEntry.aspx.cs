using System;
using System.Globalization;

namespace PraticeManagement
{
	public partial class PersonTimeEntry : System.Web.UI.Page
	{
		private const string WeekLabelFormat = "{0} {1}–{2}, {3}";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				calSelection.SelectedDate = DateTime.Today;

				// TODO: Remove the dummy code
				gvWeeklyView.DataSource = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };
				gvWeeklyView.DataBind();

				gvTimeOff.DataSource = new string[] { string.Empty };
				gvTimeOff.DataBind();
			}
		}

		protected void btnToday_Click(object sender, EventArgs e)
		{
			calSelection.SelectedDate = DateTime.Today;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			DateTime today = calSelection.SelectedDate;
			CultureInfo curentCulture = CultureInfo.CurrentUICulture;
			lblWeek.Text =
				string.Format(WeekLabelFormat,
				curentCulture.DateTimeFormat.MonthNames[today.Month],
				today.AddDays(-(double)today.DayOfWeek).Day,
				today.AddDays(7.0 - (double)today.DayOfWeek).Day,
				today.Year);
		}
	}
}

