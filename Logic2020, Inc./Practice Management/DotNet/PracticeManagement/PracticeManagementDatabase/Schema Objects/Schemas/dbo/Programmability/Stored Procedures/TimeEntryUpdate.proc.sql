-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-12-08
-- Description:	Updates time entry record
-- =============================================
CREATE PROCEDURE [dbo].[TimeEntryUpdate]
	@TimeEntryId INT,
	@EntryDate datetime,
	@MilestoneDate datetime,
	@MilestonePersonId int,
	@ActualHours real,
	@ForecastedHours real,
	@TimeTypeId int,
	@ModifiedBy int,
	@Note varchar(1000),
	@IsChargeable BIT,
	@IsReviewed BIT = NULL,
	@IsCorrect BIT,
	@DefaultMilestoneId INT,
	@PersonId INT   
AS
BEGIN
	SET NOCOUNT ON;

	declare @OldIsReviewed bit
	
	DECLARE @GMT NVARCHAR(10) = (SELECT Value FROM Settings WHERE SettingsKey = 'TimeZone')
	DECLARE @CurrentPMTime DATETIME = (CASE WHEN CHARINDEX('-',@GMT) >0 THEN GETUTCDATE() - REPLACE(@GMT,'-','') ELSE 
											GETUTCDATE() + @GMT END)

	IF @MilestonePersonId = 0 
            EXECUTE dbo.MilestonePersonEntryCreateProgrammatically @PersonId = @PersonId, --  int
                @MilestoneId = @DefaultMilestoneId, @MilestoneDate = @PersonId, --  datetime
                @ActualHours = @ActualHours, --  real
                @DefaultMpId = @DefaultMilestoneId,
                @NewMilestonePersonId = @MilestonePersonId OUTPUT

	-- To change TE status from declined according to #2110
	select @OldIsReviewed = [IsReviewed]
	from [dbo].[TimeEntries]
	WHERE TimeEntryId = @TimeEntryId
	
	UPDATE [dbo].[TimeEntries]
	   SET [EntryDate] = @EntryDate
		  ,[ModifiedDate] = @CurrentPMTime
		  ,[MilestonePersonId] = @MilestonePersonId
		  ,[ActualHours] = @ActualHours
		  ,[ForecastedHours] = @ForecastedHours
		  ,[TimeTypeId] = @TimeTypeId
		  ,[ModifiedBy] = @ModifiedBy
		  ,[Note] = @Note
		  ,[IsChargeable] = @IsChargeable
		  ,[MilestoneDate] = @MilestoneDate
		  ,[IsReviewed] = case when @OldIsReviewed = 0 and @IsReviewed = 0 then NULL else @IsReviewed end
		  ,[IsCorrect] = @IsCorrect
	WHERE TimeEntryId = @TimeEntryId
END


GO



