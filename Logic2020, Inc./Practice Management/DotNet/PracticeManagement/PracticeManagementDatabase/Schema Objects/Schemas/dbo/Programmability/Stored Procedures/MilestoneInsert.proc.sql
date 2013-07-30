﻿CREATE PROCEDURE dbo.MilestoneInsert
(
	@MilestoneId              INT OUT,
	@ProjectId                INT,
	@Description              NVARCHAR(255),
	@Amount                   DECIMAL(18,2),
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@IsHourlyAmount           BIT,
	@UserLogin                NVARCHAR(255),
	@ConsultantsCanAdjust	  BIT,
	@IsChargeable			  BIT,
	@IsDefault				  BIT = 0
)
AS
BEGIN
	SET NOCOUNT ON

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @MilestoneCount INT
	SELECT @MilestoneCount = COUNT(*) FROM dbo.Milestone WHERE ProjectId = @ProjectId

	DECLARE	@W2SalaryTimescaleId INT,
			@W2HourlyTimescaleId INT 
	SELECT	@W2SalaryTimescaleId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT  @W2HourlyTimescaleId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'

	INSERT INTO dbo.Milestone
	            (ProjectId, Description, Amount, StartDate,
	             ProjectedDeliveryDate, IsHourlyAmount, ConsultantsCanAdjust, IsChargeable, IsDefault)
	     VALUES (@ProjectId, @Description, @Amount, @StartDate,
	             @ProjectedDeliveryDate, @IsHourlyAmount, @ConsultantsCanAdjust, @IsChargeable, @IsDefault)

	SET @MilestoneId = SCOPE_IDENTITY()

	IF @MilestoneCount = 0
	BEGIN
	
		INSERT INTO dbo.Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
		SELECT P.ProjectId,AR.AttributionRecordId,2 AS AttributionRecordTypeId,P.DirectorId,
					 CASE WHEN P.StartDate > pay.StartDate THEN P.StartDate ELSE pay.StartDate END,
					 CASE WHEN P.EndDate < pay.EndDateOrig  THEN P.EndDate ELSE pay.EndDateOrig-1 END,100 AS Percentage
		FROM dbo.Project P 
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		INNER JOIN v_Pay pay ON pay.PersonId = P.DirectorId 
		WHERE P.DirectorId IS NOT NULL AND  (P.StartDate < pay.EndDateOrig) AND (pay.StartDate <= P.EndDate) AND pay.Timescale IN (@W2SalaryTimescaleId,@W2HourlyTimescaleId)
		UNION 
		SELECT  P.ProjectId,AR.AttributionRecordId,2,P.SeniorManagerId,
					 CASE WHEN P.StartDate > pay.StartDate THEN P.StartDate ELSE pay.StartDate END,
					 CASE WHEN P.EndDate < pay.EndDateOrig  THEN P.EndDate ELSE pay.EndDateOrig-1 END,100 AS Percentage
		FROM dbo.Project P 
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		INNER JOIN v_Pay pay ON pay.PersonId =  P.SeniorManagerId AND  (P.StartDate <= pay.EndDateOrig) AND (pay.StartDate <= P.EndDate)
		WHERE P.SeniorManagerId IS NOT NULL AND  (P.StartDate < pay.EndDateOrig) AND (pay.StartDate <= P.EndDate) AND pay.Timescale IN (@W2SalaryTimescaleId,@W2HourlyTimescaleId)
		UNION
		SELECT  P.ProjectId,AR.AttributionRecordId,1,P.SalesPersonId,
					 CASE WHEN P.StartDate > pay.StartDate THEN P.StartDate ELSE pay.StartDate END,
					 CASE WHEN P.EndDate < pay.EndDateOrig  THEN P.EndDate ELSE pay.EndDateOrig-1 END,100 AS Percentage
		FROM dbo.Project P
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		INNER JOIN v_Pay pay ON pay.PersonId = P.SalesPersonId AND  (P.StartDate <= pay.EndDateOrig) AND (pay.StartDate <= P.EndDate)
		WHERE P.SalesPersonId IS NOT NULL AND  (P.StartDate < pay.EndDateOrig) AND (pay.StartDate <= P.EndDate) AND pay.Timescale IN (@W2SalaryTimescaleId,@W2HourlyTimescaleId)
		UNION
		SELECT P.ProjectId,AR.AttributionRecordId,AT.AttributionTypeId,P.PracticeId,NULL,NULL,100
		FROM dbo.Project P
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsPercentageType = 1 AND P.ProjectId = @ProjectId
		CROSS JOIN dbo.AttributionTypes AT 

		

	END
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END

