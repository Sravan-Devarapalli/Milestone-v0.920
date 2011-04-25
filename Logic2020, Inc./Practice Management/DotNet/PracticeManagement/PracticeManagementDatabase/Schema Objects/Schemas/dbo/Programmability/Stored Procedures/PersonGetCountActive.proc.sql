CREATE PROCEDURE dbo.PersonGetCountActive
(
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	SELECT EmployeesNumber = dbo.GetEmployeeNumber(@StartDate, @EndDate),		
		   ConsultantsNumber = dbo.GetCounsultantsNumber(@StartDate, @EndDate)


