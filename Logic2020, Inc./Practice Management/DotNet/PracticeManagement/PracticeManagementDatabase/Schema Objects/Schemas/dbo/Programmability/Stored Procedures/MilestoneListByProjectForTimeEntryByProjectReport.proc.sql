CREATE PROCEDURE dbo.MilestoneListByProjectForTimeEntryByProjectReport
(
	@ProjectId INT
)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM DefaultMilestoneSetting WHERE ProjectId = @ProjectId)
	BEGIN
		
		DECLARE @Milestones TABLE
		(MilestoneId INT,
		 [Description] NVARCHAR(255)
		 )
		
		DECLARE @MonthStartDate DATETIME,
				@MonthEndDate   DATETIME,
				@ProjectStartDate DATETIME,
				@ProjectEndDate DATETIME,
				@CurrentMonthEnd	DATETIME,
				@DefaultMilestoneId INT,
				@TimeEnteredMaxDate DATETIME,
				@Today				DATETIME,
				@MaxDate			DATETIME
				
		SELECT  @DefaultMilestoneId=MilestoneId
		FROM dbo.DefaultMilestoneSetting
		
		SELECT @ProjectStartDate = StartDate,
				@ProjectEndDate  = EndDate
		FROM dbo.Project
		WHERE ProjectId = @ProjectId

		SELECT @MonthStartDate = @ProjectStartDate ,
		@Today = dbo.GettingPMTime(GETUTCDATE())
		SELECT @MonthEndDate =  DATEADD(MM,1,CONVERT(DATETIME, CONVERT(NVARCHAR,YEAR(@ProjectStartDate))+
													'-'+CONVERT(NVARCHAR,MONTH(@ProjectStartDate))+'-01'))-1
		SELECT @CurrentMonthEnd = DATEADD(MM,1,CONVERT(DATETIME, CONVERT(NVARCHAR,YEAR(@Today))+
													'-'+CONVERT(NVARCHAR,MONTH(@Today))+'-01'))-1

		SELECT @TimeEnteredMaxDate = MAX(MilestoneDate)
		FROM TimeEntries TE
		JOIN MilestonePerson MP ON Te.MilestonePersonId = MP.MilestonePersonId
		WHERE MP.MilestoneId = @DefaultMilestoneId
		
		SET @TimeEnteredMaxDate = DATEADD(MM,1,CONVERT(DATETIME, CONVERT(NVARCHAR,YEAR(@TimeEnteredMaxDate))+
													'-'+CONVERT(NVARCHAR,MONTH(@TimeEnteredMaxDate))+'-01'))-1
		
		SELECT @MaxDate = CASE WHEN @TimeEnteredMaxDate < @CurrentMonthEnd 
								THEN @CurrentMonthEnd 
								ELSE @TimeEnteredMaxDate 
								END
		
		WHILE (@MonthStartDate <= @MaxDate AND @ProjectEndDate >= @MaxDate)
		BEGIN
			INSERT INTO @Milestones
			SELECT CONVERT(INT,CONVERT(NVARCHAR,@MonthStartDate,112))
					,DATENAME(MM,@MonthStartDate)+' ' +DATENAME(YYYY,@MonthStartDate)
					
			SELECT @MonthStartDate = @MonthEndDate+1
			SELECT @MonthEndDate = DATEADD(MM,1,@MonthStartDate)-1
			
		END
		
		SELECT * 
		FROM @Milestones
		
		
	END
	ELSE
	BEGIN
		SELECT  
	       m.MilestoneId,
	       m.Description  
	 FROM dbo.v_Milestone AS m
	 WHERE m.ProjectId = @ProjectId
	 ORDER BY m.StartDate, m.ProjectedDeliveryDate
	END
END
