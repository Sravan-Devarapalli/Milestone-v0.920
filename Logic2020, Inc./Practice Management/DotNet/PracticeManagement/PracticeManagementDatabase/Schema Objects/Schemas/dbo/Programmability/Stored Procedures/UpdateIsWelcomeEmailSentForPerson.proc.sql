CREATE PROCEDURE [dbo].[UpdateIsWelcomeEmailSentForPerson] 
	@PersonId             INT
AS
BEGIN
	SET NOCOUNT ON;

	Update Person
	Set IsWelcomeEmailSent = 1
	Where PersonId = @PersonId
END
GO

