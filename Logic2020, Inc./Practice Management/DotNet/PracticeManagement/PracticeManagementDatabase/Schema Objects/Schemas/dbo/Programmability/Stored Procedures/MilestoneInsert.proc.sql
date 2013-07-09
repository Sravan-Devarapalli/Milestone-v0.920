CREATE PROCEDURE dbo.MilestoneInsert
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

	INSERT INTO dbo.Milestone
	            (ProjectId, Description, Amount, StartDate,
	             ProjectedDeliveryDate, IsHourlyAmount, ConsultantsCanAdjust, IsChargeable, IsDefault)
	     VALUES (@ProjectId, @Description, @Amount, @StartDate,
	             @ProjectedDeliveryDate, @IsHourlyAmount, @ConsultantsCanAdjust, @IsChargeable, @IsDefault)

	SET @MilestoneId = SCOPE_IDENTITY()

	IF @MilestoneCount = 0
	BEGIN
	
		INSERT INTO dbo.Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
		SELECT P.ProjectId,AR.AttributionRecordId,2 AS AttributionRecordTypeId,P.DirectorId,P.StartDate,P.EndDate,100 AS Percentage
		FROM dbo.Project P 
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		WHERE P.DirectorId IS NOT NULL 
		UNION ALL
		SELECT P.ProjectId,AR.AttributionRecordId,2,P.SeniorManagerId,P.StartDate,P.EndDate,100
		FROM dbo.Project P 
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		WHERE P.SeniorManagerId IS NOT NULL
		UNION ALL
		SELECT P.ProjectId,AR.AttributionRecordId,1,P.SalesPersonId,P.StartDate,P.EndDate,100
		FROM dbo.Project P
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsRangeType = 1 AND P.ProjectId = @ProjectId
		WHERE P.SalesPersonId IS NOT NULL
		UNION ALL
		SELECT P.ProjectId,AR.AttributionRecordId,AT.AttributionTypeId,P.PracticeId,NULL,NULL,100
		FROM dbo.Project P
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsPercentageType = 1 AND P.ProjectId = @ProjectId
		CROSS JOIN dbo.AttributionTypes AT 

		

	END
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
