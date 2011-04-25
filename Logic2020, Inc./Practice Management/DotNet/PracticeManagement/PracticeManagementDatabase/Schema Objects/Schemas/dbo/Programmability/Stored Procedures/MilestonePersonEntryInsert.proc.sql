-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-09-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-27-2008
-- Update Date: 10-27-2010
-- Updated By: Ravi Narsini (Changes: Adding logic for #2600. Needs to add person entry automatically for project's default milestone.)
-- Description:	Inserts person-milestone details for the specified milestone and person.
-- =============================================
CREATE PROCEDURE dbo.MilestonePersonEntryInsert
(
	@PersonId            INT = NULL,
	@MilestonePersonId   INT,
	@StartDate     DATETIME,
	@EndDate       DATETIME,
	@HoursPerDay   DECIMAL(4,2),
	@PersonRoleId  INT,
	@Amount        DECIMAL(18,2),
	@Location      NVARCHAR(20) = NULL,
	@UserLogin     NVARCHAR(255)
)
AS
	SET NOCOUNT ON

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	INSERT INTO dbo.MilestonePersonEntry
	            (MilestonePersonId, StartDate, EndDate, PersonRoleId, Amount, HoursPerDay, Location)
	     VALUES (@MilestonePersonId, @StartDate, @EndDate, @PersonRoleId, @Amount, @HoursPerDay, @Location)
	     
	IF @PersonId IS NOT NULL
	BEGIN 
		UPDATE dbo.MilestonePerson
		SET PersonId = @PersonId
		WHERE MilestonePersonId = @MilestonePersonId
	END 
	
	-- End logging session
	EXEC dbo.SessionLogUnprepare

