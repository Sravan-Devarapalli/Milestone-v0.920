-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-12-08
-- Description:	Inserts new time entry record
-- =============================================
CREATE PROCEDURE [dbo].[TimeEntryInsert]
    @TimeEntryId INT OUT ,
    @EntryDate DATETIME ,
    @MilestoneDate DATETIME ,
    @MilestonePersonId INT ,
    @ActualHours REAL ,
    @ForecastedHours REAL ,
    @TimeTypeId INT ,
    @ModifiedBy INT ,
    @Note VARCHAR(1000) ,
	@IsChargeable BIT,
    @DefaultMpId INT ,
    @IsCorrect BIT ,
    @PersonId INT -- id of the person that this milestone is about
				  -- it's needed when we're assiging person to the default
				  --	milestone, so it's not possible to extract id
				  --	from the milestonepersonid parameter
AS 
    BEGIN
        SET NOCOUNT ON ;
	
        BEGIN TRANSACTION
		
		DECLARE @CurrentPMTime DATETIME 
		SET @CurrentPMTime = dbo.InsertingTime()
		
        --DECLARE @IsChargeable BIT
	
	-- If it's default milestone, create MPE for it
        IF @MilestonePersonId = 0 
            EXECUTE dbo.MilestonePersonEntryCreateProgrammatically @PersonId = @PersonId, --  int
                @MilestoneId = @DefaultMpId, @MilestoneDate = @PersonId, --  datetime
                @ActualHours = @ActualHours, --  real
                @DefaultMpId = @DefaultMpId,
                @NewMilestonePersonId = @MilestonePersonId OUTPUT
	
	--	Fill forecasted hours field if not present
        IF ( @ForecastedHours = 0 ) 
            BEGIN
                SELECT  @ForecastedHours = mpe.HoursPerDay
	--                  , @IsChargeable = m.IsChargeable
                FROM    dbo.MilestonePersonEntry AS mpe
                        INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
                        INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
                WHERE   mpe.MilestonePersonId = @MilestonePersonId
                        AND @MilestoneDate BETWEEN mpe.StartDate
                                           AND     ISNULL(mpe.EndDate,
                                                          dbo.GetFutureDate())
            END

	-- Check if the milestone date is inside corresponding milestone 
	--	start and end date interval
	IF NOT EXISTS (
					SELECT 1 
					FROM dbo.MilestonePersonEntry mpe
					WHERE mpe.MilestonePersonId = @MilestonePersonId
							AND @MilestoneDate BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, '2/3/2099'))
	BEGIN 
		DECLARE @MStart VARCHAR(20)
		DECLARE @MEnd VARCHAR(20)
		
		SELECT @MStart = CONVERT(VARCHAR, mpe.StartDate, 101), 
				 @MEnd = CONVERT(VARCHAR, mpe.EndDate, 101)
		FROM dbo.MilestonePersonEntry mpe
		WHERE mpe.MilestonePersonId = @MilestonePersonId	
		
		RAISERROR (N'The date you record time entry for should be between %s and %s.',
					16, 1, @MStart, @MEnd)
	END 
	ELSE 
        INSERT  INTO [dbo].[TimeEntries]
                ( [EntryDate] ,
                  [MilestoneDate] ,
                  [ModifiedDate] ,
                  [MilestonePersonId] ,
                  [ActualHours] ,
                  [ForecastedHours] ,
                  [TimeTypeId] ,
                  [ModifiedBy] ,
                  [Note] ,
                  [IsChargeable] ,
                  [IsCorrect]
                )
        VALUES  ( @CurrentPMTime ,
                  @MilestoneDate ,
                  @CurrentPMTime ,
                  @MilestonePersonId ,
                  @ActualHours ,
                  @ForecastedHours ,
                  @TimeTypeId ,
                  @ModifiedBy ,
                  @Note ,
                  @IsChargeable ,
                  @IsCorrect
                )
				
        SET @TimeEntryId = SCOPE_IDENTITY()				
				
        COMMIT 
    END


GO



