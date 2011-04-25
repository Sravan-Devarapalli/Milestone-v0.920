using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.CalendarService;

namespace PraticeManagement.Controls
{
	public partial class MonthCalendar : System.Web.UI.UserControl
	{
		#region Constants

		private const string YearKey = "Year";
		private const string MonthKey = "Month";
		private const string PersonIdKey = "PersonId";
		private const string ValueArgument = "value";
		
		#endregion

		#region Fields

		private bool wasChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Gest or sets a year to be displayed.
		/// </summary>
		public int Year
		{
			get
			{
				return Convert.ToInt32(ViewState[YearKey]);
			}
			set
			{
				if (value < DateTime.MinValue.Year || value > DateTime.MaxValue.Year)
				{
					throw new ArgumentException(
						string.Format(Resources.Messages.InvalidYearNumber,
						DateTime.MinValue.Year,
						DateTime.MaxValue.Year),
						ValueArgument);
				}

				wasChanged = wasChanged || Year != value;
				ViewState[YearKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets a month to be displayed.
		/// </summary>
		public int Month
		{
			get
			{
				return Convert.ToInt32(ViewState[MonthKey]);
			}
			set
			{
				if (value < 1 || value > 12)
				{
					throw new ArgumentException(
						string.Format(Resources.Messages.InvalidMonthNumber, 1, 12),
						ValueArgument);
				}

				wasChanged = wasChanged || Month != value;
				ViewState[MonthKey] = value;
			}
		}

		public int? PersonId
		{
			get
			{
				return (int?)ViewState[PersonIdKey];
			}
			set
			{
				wasChanged = wasChanged || PersonId != value;
				ViewState[PersonIdKey] = value;
			}
		}

		public CalendarItem[] CalendarItems
		{
			private get;
			set;
		}

		#endregion

		#region Methods

		protected string DayOnClientClick()
		{
			return string.Format("updatingCalendarContainer = $get('{0}');", lstCalendar.ClientID);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		
			if (!IsPostBack || wasChanged)
			{
				Display();
			}
		}

		private void Display()
		{
			if (CalendarItems != null)
			{
				DateTime firstMonthDay = new DateTime(Year, Month, 1);
				DateTime lastMonthDay = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));

				DateTime firstDisplayedDay = firstMonthDay.AddDays(-(double)firstMonthDay.DayOfWeek);
				DateTime lastDisplayedDay = lastMonthDay.AddDays(6.0 - (double)lastMonthDay.DayOfWeek);

				CalendarItem[] itemsToDisplay = Array.FindAll<CalendarItem>(CalendarItems, delegate(CalendarItem item)
					{
						return item.Date >= firstDisplayedDay && item.Date <= lastDisplayedDay;
					});
				lstCalendar.DataSource = itemsToDisplay;
				lstCalendar.DataBind();
			}
		}

		protected void btnDay_Command(object sender, CommandEventArgs e)
		{
			CalendarItem item = new CalendarItem();
			item.Date = DateTime.Parse((string)e.CommandArgument);
			item.DayOff = bool.Parse(e.CommandName);
			item.PersonId = PersonId;

			SaveDate(item);
			Display();
		}

		private void SaveDate(CalendarItem item)
		{
			using (CalendarServiceClient serviceClient = new CalendarServiceClient())
			{
				try
				{
					serviceClient.SaveCalendar(item);
				}
				catch (FaultException<ExceptionDetail>)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		#endregion
	}
}
