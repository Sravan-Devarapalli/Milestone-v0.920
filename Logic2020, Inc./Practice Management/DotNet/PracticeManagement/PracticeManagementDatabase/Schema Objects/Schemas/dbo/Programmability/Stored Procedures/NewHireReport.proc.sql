CREATE PROCEDURE [dbo].[NewHireReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@PersonStatusIds	XML,
	@TimeScaleIds	XML,
	@PracticeIds	XML,
	@ExcludeInternalPractices	BIT,
	@PersonDivisionIds	XML,
	@SeniorityIds	XML,
	@HireDates	XML,
	@RecruiterIds	XML
)
AS
BEGIN
	
	SELECT *
	FROM dbo.Person P
	INNER JOIN dbo.Pay P ON P.PersonId = P.PersonId
	INNER JOIN dbo.RecruiterCommission RC ON RC.RecruitId = P.PersonId

END
