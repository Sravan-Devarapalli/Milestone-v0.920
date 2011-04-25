
namespace DataTransferObjects
{
	/// <summary>
	/// Determines the custom error codes used by the system.
	/// </summary>
	public enum ErrorCode
	{
		/// <summary>
		/// The error code cannot be resolved.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The Person name uniquness
		/// </summary>
		PersonNameUniquenesViolation = 70001,

		/// <summary>
		/// The Person email uniqueness
		/// </summary>
		PersonEmailUniquenesViolation = 70002,

		/// <summary>
		/// The Recruiting commission period
		/// </summary>
		RecruitingCommissionPeriodViolation = 70003,

		/// <summary>
		/// Client Name uniquness
		/// </summary>
		ClientNameUniquenesViolation = 70004,

		/// <summary>
		/// The Start Date of the compensation is incorrect.
		/// </summary>
		CompensationStartDateIncorrect = 70005,

		/// <summary>
		/// Cannot inactivate the person who takes part in active milestones.
		/// </summary>
		CannotInactivatePerson = 70006,

		/// <summary>
		/// The End Date is incorrect.
		/// </summary>
		EndDateIncorrect = 70007,

		/// <summary>
		/// Person Compensation period incorrect.
		/// </summary>
		PeriodIncorrect = 70008,

		/// <summary>
		/// The Person employee number uniqueness
		/// </summary>
		PersonEmployeeNumberUniquenesViolation = 70009,

		/// <summary>
		/// The Expense Category cannot be deleted because it is in use.
		/// </summary>
		ExpenseCategoryIsInUse = 70010,

		/// <summary>
		/// The specified expense category does not exist anymore.
		/// </summary>
		ExpenseCategoryDeleted = 70012,

		/// <summary>
		/// The Start Date of the default recruiter commission is incorrect.
		/// </summary>
		DefaultRecruiterCommissionStartDateIncorrect = 70013,

		/// <summary>
		/// The End Date of the default recruiter commission is incorrect.
		/// </summary>
		DefaultRecruiterCommissionEndDateIncorrect = 70014,

		/// <summary>
		/// The period of the default recruiter commission is incorrect.
		/// </summary>
		DefaultRecruiterCommissionPeriodIncorrect = 70015,

		/// <summary>
		/// The specified person is already assigned on this milestone.
		/// </summary>
        MilestonePersonAlreadyExists = 70016,

        /// <summary>
        /// This milestone cannot be deleted, because there are time entries related to it.
        /// </summary>
        MilestoneCannotBeDeleted = 70017
	}
}

