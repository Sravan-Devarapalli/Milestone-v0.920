using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Contains the data for the person stats report.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PersonStats
	{
		#region Fields

		private PracticeManagementCurrency revenueValue;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a financial date.
		/// </summary>
		[DataMember]
		public DateTime Date
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a revenue.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency Revenue
		{
			get
			{
				return revenueValue;
			}
			set
			{
				revenueValue = value;
				revenueValue.FormatStyle = NumberFormatStyle.Revenue;
			}
		}

		/// <summary>
		/// Gets or sets the admin costs.
		/// </summary>
		[DataMember]
		public PracticeManagementCurrency AdminCosts
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of the employees.
		/// </summary>
		[DataMember]
		public int EmployeesCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a number of the consultants.
		/// </summary>
		[DataMember]
		public int ConsultantsCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets "all_consulting_hours divided by working_days_in_month divided by 8".
		/// </summary>
		[DataMember]
		public decimal VirtualConsultants
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a ratio of the VirtualConsultants to the VirtualConsultants from a prev month.
		/// </summary>
		[DataMember]
		public decimal VirtualConsultantsChange
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a revenue per employee.
		/// </summary>
		public PracticeManagementCurrency RevenuePerEmployee
		{
			get
			{
				PracticeManagementCurrency result =
					EmployeesCount != 0 ? (decimal)Revenue / EmployeesCount : 0M;
				result.FormatStyle = NumberFormatStyle.Revenue;
				return result;
			}
		}

		/// <summary>
		/// Gets a revenue per consultant.
		/// </summary>
		public PracticeManagementCurrency RevenuePerConsultant
		{
			get
			{
				PracticeManagementCurrency result =
					ConsultantsCount != 0 ? Revenue / ConsultantsCount : new PracticeManagementCurrency();
				result.FormatStyle = NumberFormatStyle.Revenue;
				return result;
			}
		}

		/// <summary>
		/// Gets a ratio the admin costs to the revenue.
		/// </summary>
		public decimal AdminRevenueRatio
		{
			get
			{
				return AdminCosts / Revenue;
			}
		}

		#endregion
	}
}

