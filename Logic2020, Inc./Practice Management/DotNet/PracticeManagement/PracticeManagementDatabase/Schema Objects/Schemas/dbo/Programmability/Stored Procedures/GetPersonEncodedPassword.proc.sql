CREATE PROCEDURE [dbo].[GetPersonEncodedPassword]
	@personId int
AS
BEGIN
    SELECT password
	FROM PersonPassword
	WHERE personId = @personId
END
