CREATE PROCEDURE [dbo].[DeleteStrawman]
(
	@PersonId	INT,
	@UserLogin	NVARCHAR(255)	 
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN DeleteStrawman
		EXEC dbo.SessionLogPrepare @userLogin = @UserLogin
		
		DELETE MilestonePerson
		WHERE personid = @PersonId

		--Deleting the Strawman
		DELETE P
		FROM dbo.Person P
		WHERE P.PersonId = @PersonId AND P.IsStrawman = 1

		EXEC dbo.SessionLogUnprepare
		COMMIT TRAN DeleteStrawman
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN DeleteStrawman
		DECLARE @Error NVARCHAR(2000)
		SET @Error = ERROR_MESSAGE()
		RAISERROR(@Error, 16, 1)
	END CATCH
END

