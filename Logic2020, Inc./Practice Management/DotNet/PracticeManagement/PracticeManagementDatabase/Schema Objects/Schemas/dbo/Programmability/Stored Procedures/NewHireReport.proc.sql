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
	
	SELECT P.PersonId,
			P.FirstName,
			P.LastName,
			Ps.PersonStatusId,
			PS.Name AS PersonStatusName,
			TS.TimescaleId AS Timescale,
			TS.Name AS TimescaleName,
			RC.RecruiterId,
			RCP.FirstName AS RecruiterFirstName ,
			RCP.LastName RecruiterLastName,
			S.SeniorityId,
			S.Name AS SeniorityName,
			P.DivisionId,
			P.HireDate
	FROM dbo.Person P
	INNER JOIN dbo.Pay Pay ON P.PersonId = Pay.Person
	INNER JOIN dbo.Timescale TS ON TS.TimescaleId = Pay.Timescale
	INNER JOIN dbo.RecruiterCommission RC ON RC.RecruitId = P.PersonId
	INNER JOIN dbo.Person RCP ON RC.RecruiterId = RCP.PersonId
	INNEr JOIN dbo.PersonStatus PS ON PS.PersonStatusId = P.PersonStatusId
	INNER JOIN dbo.Seniority S ON S.[SeniorityId] = Pay.[SeniorityId]

END
