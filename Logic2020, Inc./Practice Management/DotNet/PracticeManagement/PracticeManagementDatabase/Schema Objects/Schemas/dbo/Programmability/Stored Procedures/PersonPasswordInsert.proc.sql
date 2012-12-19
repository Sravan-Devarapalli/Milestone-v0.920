CREATE PROCEDURE [dbo].[PersonPasswordInsert]
	@personId int, 
	@encodedPassword NVARCHAR(128)
AS
BEGIN
	IF EXISTS (SELECT personId FROM PersonPassword WHERE personId = @personId)
	BEGIN
		UPDATE PersonPassword
		SET password = @encodedPassword
		WHERE personId = @personId
	END
	ELSE
	BEGIN
	INSERT INTO PersonPassword
				(personId
				,password)
			VALUES
				(@personId
				,@encodedPassword)
	END
END

