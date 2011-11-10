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
	@UserLogin     NVARCHAR(255),
	@Id            INT = NULL OUTPUT 
)
AS
	SET NOCOUNT ON

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin



	INSERT INTO dbo.MilestonePersonEntry
	            (MilestonePersonId, StartDate, EndDate, PersonRoleId, Amount, HoursPerDay, Location)
	     VALUES (@MilestonePersonId, @StartDate, @EndDate, @PersonRoleId, @Amount, @HoursPerDay, @Location)

		SET @Id = SCOPE_IDENTITY()
	     
	IF @PersonId IS NOT NULL
	BEGIN 
		UPDATE dbo.MilestonePerson
		SET PersonId = @PersonId
		WHERE MilestonePersonId = @MilestonePersonId
	END 

	-- End logging session
	EXEC dbo.SessionLogUnprepare




