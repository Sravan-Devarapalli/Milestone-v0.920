CREATE PROCEDURE [dbo].[IsProjectAttributionConflictsWithMilestoneChanges]
	(
		@MilestoneId	INT,
		@StartDate		DATETIME,
		@EndDate		DATETIME,
		@IsUpdate		BIT = 1
	)
AS
BEGIN
		DECLARE @ProjectStartDate	DATETIME,-- @ProjectStartDate,@ProjectEndDate are resultant project startdate and enddates on update and delete.
				@ProjectEndDate		DATETIME,
				@ProjectId			INT
		SELECT	@ProjectId = ProjectId FROM dbo.Milestone WHERE MilestoneId = @MilestoneId

		SELECT	@ProjectStartDate = MIN(M.StartDate),
					@ProjectEndDate = MAX(M.ProjectedDeliveryDate)
		FROM	dbo.Milestone M
		WHERE	M.ProjectId = @ProjectId AND M.MilestoneId <> @MilestoneId
		GROUP BY M.ProjectId

		IF @IsUpdate = 1
		BEGIN	
				SELECT	@ProjectStartDate = CASE WHEN @ProjectStartDate < @StartDate THEN  @ProjectStartDate ELSE  @StartDate END,
						@ProjectEndDate = CASE WHEN @ProjectEndDate > @EndDate THEN  @ProjectEndDate ELSE  @EndDate END
		END

		SELECT	A.AttributionId,A.StartDate,A.EndDate,A.AttributionTypeId,A.TargetId,P.LastName+', '+P.FirstName AS TargetName
		FROM	dbo.Attribution A
		JOIN    dbo.Person P ON P.PersonId = A.TargetId 
		WHERE	A.ProjectId = @ProjectId AND (A.StartDate < @ProjectStartDate OR  A.EndDate > @ProjectEndDate)
END
