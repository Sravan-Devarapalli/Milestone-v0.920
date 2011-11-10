CREATE PROCEDURE [dbo].[DeleteMilestonePersonEntry]
(
	@Id   INT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN

-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	SET NOCOUNT ON;

	DECLARE @MilestonePersonId   INT

	SELECT @MilestonePersonId = MilestonePersonId
	FROM dbo.MilestonePersonEntry
	WHERE Id = @Id
	
	DELETE 
	FROM dbo.MilestonePersonEntry
	WHERE Id = @Id

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
