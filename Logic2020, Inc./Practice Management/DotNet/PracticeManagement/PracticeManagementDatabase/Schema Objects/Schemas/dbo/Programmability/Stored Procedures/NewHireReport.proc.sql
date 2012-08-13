CREATE PROCEDURE [dbo].[NewHireReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@PersonStatusIds	XML = null,
	@TimeScaleIds	XML = null,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT,
	@PersonDivisionIds	XML = null,
	@SeniorityIds	XML = null,
	@HireDates	XML = null,
	@RecruiterIds	XML = null
)
AS
BEGIN
	
	SELECT *,PS.Name AS PersonStatusName,TS.Name AS TimescaleName,
	S.Name AS SeniorityName,'RecruiterFirstName' AS RecruiterFirstName ,'RecruiterLastName' as RecruiterLastName,
	P.DivisionId
	FROM dbo.Person P
	INNER JOIN dbo.Pay Pay ON P.PersonId = Pay.Person
	INNER JOIN dbo.Timescale TS ON TS.TimescaleId = Pay.Timescale
	INNER JOIN dbo.RecruiterCommission RC ON RC.RecruitId = P.PersonId
	INNEr JOIN dbo.PersonStatus PS ON PS.PersonStatusId = P.PersonStatusId
	INNER JOIN dbo.Seniority S ON S.[SeniorityId] = Pay.[SeniorityId]

END
