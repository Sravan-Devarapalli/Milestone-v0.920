CREATE PROCEDURE  [dbo].[IsPersonAlreadyHavingStatus]
@PersonStatusId       INT,
@PersonId             INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT COUNT(*) as [Count]
	FROM PersonStatusHistory AS psh
	WHERE psh.PersonId = @PersonId AND psh.PersonStatusId =@PersonStatusId
	
END
GO
