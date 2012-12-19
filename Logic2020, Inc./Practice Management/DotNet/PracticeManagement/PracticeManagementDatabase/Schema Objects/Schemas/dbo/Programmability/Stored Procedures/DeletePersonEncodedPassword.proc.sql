CREATE PROCEDURE [dbo].[DeletePersonEncodedPassword]
	@personId int
AS
BEGIN
   DELETE FROM PersonPassword
   WHERE personId = @personId
END
