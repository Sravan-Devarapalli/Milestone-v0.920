CREATE PROCEDURE [dbo].[UpdatePersonDivision]
	@DivisionId INT, 
	@DivisionOwnerId INT
AS
BEGIN
		UPDATE dbo.PersonDivision
		SET DivisionOwnerId = @DivisionOwnerId
		WHERE DivisionId=@DivisionId
END
