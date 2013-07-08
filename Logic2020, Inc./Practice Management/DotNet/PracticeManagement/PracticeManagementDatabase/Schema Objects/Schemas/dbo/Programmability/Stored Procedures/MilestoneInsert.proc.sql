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
		SELECT P.ProjectId,1,2,P.DirectorId,P.StartDate,P.EndDate,100
		FROM dbo.Project P 
		WHERE P.ProjectId = @ProjectId AND P.DirectorId IS NOT NULL 

		INSERT INTO dbo.Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
		SELECT P.ProjectId,1,2,P.SeniorManagerId,P.StartDate,P.EndDate,100
		FROM dbo.Project P 
		WHERE P.ProjectId = @ProjectId AND P.SeniorManagerId IS NOT NULL

		INSERT INTO dbo.Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
		SELECT P.ProjectId,1,1,P.SalesPersonId,P.StartDate,P.EndDate,100
		FROM dbo.Project P
		WHERE P.ProjectId = @ProjectId AND P.SalesPersonId IS NOT NULL
		
		INSERT INTO dbo.Attribution(ProjectId,AttributionRecordTypeId,AttributionTypeId,TargetId,StartDate,EndDate,Percentage)
		SELECT P.ProjectId,AR.AttributionRecordId,AT.AttributionTypeId,P.PracticeId,NULL,NULL,100
		FROM dbo.Project P
		INNER JOIN dbo.AttributionRecordTypes AR ON AR.IsPercentageType = 1 AND P.ProjectId = @ProjectId
		CROSS JOIN dbo.AttributionTypes AT 

		

	END
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
