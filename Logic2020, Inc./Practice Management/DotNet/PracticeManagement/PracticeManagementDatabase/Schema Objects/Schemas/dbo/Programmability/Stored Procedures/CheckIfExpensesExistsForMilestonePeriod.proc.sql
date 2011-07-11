CREATE PROCEDURE dbo.CheckIfExpensesExistsForMilestonePeriod
(
	@MilestoneId	INT,
	@StartDate		DATETIME = NULL,
	@EndDate		DATETIME = NULL
)
AS
  DECLARE 
  --@ProjectStartDate DATETIME,
		--	@ProjectEndDate	DATETIME,
			--@MilestoneOldStartDate	DATETIME,
			--@MilestoneOldEndDate DATETIME,
			@ProjectNewStartDate DATETIME,
			@ProjectNewEndDate	DATETIME,
			@ProjectId	INT

		SELECT 
		--@MilestoneOldStartDate = M.StartDate,
		--		@MilestoneOldEndDate = M.ProjectedDeliveryDate,
				--@ProjectStartDate = P.StartDate,
				--@ProjectEndDate = P.EndDate,
				@ProjectId = M.ProjectId
		FROM Milestone M
		JOIN Project P ON M.ProjectId = P.ProjectId
		WHERE M.MilestoneId = @MilestoneId
	SELECT  @ProjectNewStartDate = MIN(StarTDate),
			@ProjectNewEndDate	=MAX(ProjectedDeliveryDate)
	FROM Milestone 
	WHERE MilestoneId <> @MilestoneId AND ProjectId = @ProjectId
	
	IF(@StartDate IS NOT NULL AND @EndDate IS NULL)
	BEGIN
		
		IF EXISTS (SELECT 1 FROM ProjectExpense
					WHERE  
						((@ProjectNewStartDate IS NOT NULL
							AND( StartDate < @ProjectNewStartDate AND @ProjectNewStartDate < @StartDate
							OR 
								StartDate < @StartDate AND  @ProjectNewStartDate > @StartDate)
						)
					OR 
					 (StartDate < @StartDate
						AND @ProjectNewStartDate IS NULL
					 )) AND  ProjectId =@ProjectId
				 )
		BEGIN			
			SELECT 'True'
		END
		ELSE
		BEGIN
			 SELECT 'False'
		END 
	END
	ELSE IF (@StartDate IS NULL AND @EndDate IS NOT NULL)
	BEGIN
		
		IF EXISTS (SELECT 1 FROM ProjectExpense
					WHERE  
						((@ProjectNewEndDate IS NOT NULL
							AND( EndDate > @ProjectNewEndDate AND @ProjectNewEndDate > @EndDate
							OR 
								EndDate > @EndDate AND  @ProjectNewEndDate < @EndDate)
						)
					OR 
					 (EndDate > @EndDate
						AND @ProjectNewEndDate IS NULL
					 )) AND ProjectId =@ProjectId
				 )
		BEGIN
			SELECT 'True'
		END
		ELSE
		BEGIN
			SELECT 'False'
		END
	END
	ELSE IF @StartDate IS NULL AND @EndDate IS  NULL
	BEGIN
		IF EXISTS (SELECT 1 FROM ProjectExpense
					WHERE  ProjectId = @ProjectId
					AND ((StartDate < @ProjectNewStartDate OR EndDate > @ProjectNewEndDate)
						 OR (@ProjectNewStartDate IS NULL AND @ProjectNewEndDate IS NULL)
						 )
					)
		BEGIN
			SELECT 'True'
		END
		ELSE 
		BEGIN
			SELECT 'False'
		END
	END
 
