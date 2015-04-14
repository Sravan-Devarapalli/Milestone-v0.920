CREATE PROCEDURE [dbo].[GetActivePersonsByProjectId]
(
	@ProjectId	INT
)
AS
BEGIN
  
  DECLARE	@W2SalaryId			INT,
			@W2HourlyId			INT,
			@ProjectStartDate	DATETIME,
			@ProjectEndDate		DATETIME

  SELECT	@W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
  SELECT	@W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
  SELECT    @ProjectStartDate = StartDate,@ProjectEndDate = EndDate FROM dbo.Project WHERE ProjectId = @ProjectId
   
  SELECT	DISTINCT P.PersonId,
			P.FirstName,
			P.LastName
  FROM		dbo.Person P 
  INNER JOIN v_Pay pay ON pay.PersonId = P.PersonId AND  (@ProjectStartDate < pay.EndDateOrig) AND (pay.StartDate <= @ProjectEndDate)
  INNER JOIN v_DivisionHistory DH ON DH.PersonId = P.PersonId AND (DH.EndDate IS NULL OR @ProjectStartDate < DH.EndDate) AND (DH.StartDate <= @ProjectEndDate) AND ISNULL(DH.DivisionId,0) IN (SELECT DivisionId FROM dbo.PersonDivision WHERE DivisionName IN ('Consulting','Business Development','Technology Consulting','Business Consulting','Director'))
  WHERE	    P.IsStrawman = 0 --Active and Terminated Status
			AND pay.Timescale IN (@W2SalaryId,@W2HourlyId) 
END

