CREATE PROCEDURE [dbo].[ZeroHourlyRateExceptionReport]
	(
		@StartDate	DATETIME,
		@EndDate	DATETIME
	)
AS
BEGIN
	
	DECLARE @W2Hourly	INT,
			@W2Salary	INT,
			@1099Hourly	INT
	
	SELECT @W2Hourly = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
	SELECT @W2Salary = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT @1099Hourly = TimescaleId FROM dbo.Timescale WHERE Name = '1099 Hourly'

	SELECT	P.PersonId,
			P.EmployeeNumber,
			P.IsOffshore,
			P.FirstName,
			P.LastName,
			pay.TimeScaleName,
			Pro.ProjectId,
			Pro.ProjectNumber,
			Pro.Name AS ProjectName,
			Pro.ProjectStatusId,
			M.MilestoneId,
			M.Description AS MilestoneName,
			M.StartDate,
			M.ProjectedDeliveryDate,
			ISNULL(MPE.Amount,0) AS Amount,
			PS.Name AS ProjectStatus
	FROM dbo.Person P
	INNER JOIN dbo.MilestonePerson MP ON MP.PersonId = P.PersonId
	INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = M.ProjectId
	INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId
	INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = Pro.ProjectStatusId
	WHERE M.IsHourlyAmount = 1 AND MPE.StartDate <= @EndDate AND @StartDate <= MPE.EndDate
	AND pay.StartDate <= MPE.EndDate AND MPE.StartDate < pay.EndDateOrig
	AND P.IsStrawman = 0 AND Pro.ProjectStatusId IN (3,4) --Active AND Completed status
	AND pay.Timescale IN (@W2Hourly,@W2Salary,@1099Hourly)
	AND ISNULL(MPE.Amount,0) = 0 
	AND Pro.ProjectNumber != 'P031000'
	ORDER BY P.LastName,P.FirstName
END
