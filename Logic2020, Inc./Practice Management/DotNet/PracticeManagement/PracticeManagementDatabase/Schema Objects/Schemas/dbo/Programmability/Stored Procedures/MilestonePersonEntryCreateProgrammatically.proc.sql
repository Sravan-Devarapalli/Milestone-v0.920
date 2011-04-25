-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-01-14
-- Description:	Creates milestone person entry
-- =============================================
CREATE PROCEDURE dbo.MilestonePersonEntryCreateProgrammatically
	@PersonId INT,
	@MilestoneId INT, 	
	@MilestoneDate DATETIME,
	@ActualHours REAL,
	@DefaultMpId INT,
	@NewMilestonePersonId INT OUTPUT  
AS
BEGIN
	SET NOCOUNT ON;

	/*-- Needed for debugging
	DECLARE @PersonId INT
	DECLARE @MilestoneId INT
	DECLARE @MilestoneDate DATETIME
	DECLARE @ActualHours REAL
	DECLARE @NewMilestonePersonId INT 

	SET @PersonId = 3579
	SET @MilestoneId = 877
	SET @MilestoneDate = N'01/15/2010'
	SET @ActualHours = 8.00*/

	
	DECLARE @MpCount INT 
	SET @MpCount = 0
	
	--	Check if there's already a MP for this person (@MpCount then will be 0)
	--		otherwise, save corresponding MpId to the variable
	SELECT @MpCount = COUNT(*), @NewMilestonePersonId = mp.MilestonePersonId
	FROM dbo.MilestonePerson AS mp
	WHERE mp.MilestoneId = @MilestoneId AND mp.PersonId = @PersonId
	GROUP BY mp.MilestonePersonId
	
	--PRINT @MpCount
	--PRINT @NewMilestonePersonId
	
	-- If there's no MP, create one and save it's Id to the variable
	IF (@MpCount = 0)
		EXECUTE dbo.MilestonePersonInsert
					@MilestoneId = @MilestoneId, --  int
					@PersonId = @PersonId, --  int
					@MilestonePersonId = @NewMilestonePersonId OUTPUT
					
	
	--	Check if there's already a MPE for this person
	--		otherwise, save corresponding MpId to the variable
	SELECT @MpCount = COUNT(*)
	FROM dbo.MilestonePersonEntry AS mpe
	WHERE mpe.MilestonePersonId = @NewMilestonePersonId
	
	IF (@MpCount = 0)
	BEGIN 
		DECLARE @MStart DATETIME
		DECLARE @MEnd DATETIME
		DECLARE @UserAlias VARCHAR(50)
		
		SELECT @MStart = m.StartDate FROM dbo.Milestone AS m WHERE m.MilestoneId = @MilestoneId
		SELECT @MEnd = m.ProjectedDeliveryDate FROM dbo.Milestone AS m WHERE m.MilestoneId = @MilestoneId						
		SELECT @UserAlias = p.Alias FROM dbo.Person AS p WHERE p.PersonId = @PersonId
		
		--PRINT @MStart
		--PRINT @MEnd
		--PRINT @UserAlias
						
		-- Create new MPE
		EXECUTE dbo.MilestonePersonEntryInsert
				@MilestonePersonId = @NewMilestonePersonId,
				@StartDate = @MStart, --  only for milestone date
				@EndDate = @MEnd, --  only for milestone date
				@HoursPerDay = @ActualHours, --  actual hours reported
				@Amount = 0, --  not paid
				@PersonRoleId = NULL,
				@UserLogin = @UserAlias --  no user, system generated			
	END				
END

