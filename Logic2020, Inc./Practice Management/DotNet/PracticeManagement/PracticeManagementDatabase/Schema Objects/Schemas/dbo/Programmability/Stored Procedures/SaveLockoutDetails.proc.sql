CREATE PROCEDURE [dbo].[SaveLockoutDetails]
(
	@LockoutXML		XML
)
AS
BEGIN

SET ANSI_NULLS ON;
	SET NOCOUNT ON;
	/*
	<Lockouts>
		<LockoutPage LockoutPageId="1">
			<Lockout LockoutId="" FunctionalityName="" Lockout="" LockoutDate="">
			</Lockout>
        <LockoutPage>
		<LockoutPage LockoutPageId="2">
			<Lockout LockoutId="" FunctionalityName="" Lockout="" LockoutDate="">
			</Lockout>
        <LockoutPage>
		<LockoutPage LockoutPageId="3">
			<Lockout LockoutId="" FunctionalityName="" Lockout="" LockoutDate="">
			</Lockout>
        <LockoutPage>
		<LockoutPage LockoutPageId="4">
			<Lockout LockoutId="" FunctionalityName="" Lockout="" LockoutDate="">
			</Lockout>
        <LockoutPage>
	</Lockouts>
	*/
	BEGIN TRY
			BEGIN TRAN LockoutDetails

		DECLARE @LockoutValues TABLE(LockoutId				INT			NOT NULL,
									 LockoutPageId			INT			NOT NULL,
									 FunctionalityName		NVARCHAR(50)	NOT NULL,
									 Lockout				BIT			NOT NULL,
									 LockoutDate			DATETIME	NULL)

            INSERT INTO @LockoutValues
			SELECT	NEW.c.value('@LockoutId', 'INT'),
					NEW.c.value('..[1]/@LockoutPageId', 'INT'),
					NEW.c.value('@FunctionalityName', 'NVARCHAR(50)'),
					NEW.c.value('@Lockout', 'BIT'),
					NEW.c.value('@LockoutDate', 'DATETIME')
			FROM @LockoutXML.nodes('Lockouts/LockoutPage/Lockout') NEW(c)
		
			--Update values in Lockout table if LockoutId in both the table and XML matches.
			UPDATE	L 
			SET		L.Lockout = LV.Lockout,
					L.LockoutDate = CASE WHEN LV.LockoutDate = '19000101' THEN NULL ELSE LV.LockoutDate END
			FROM	dbo.Lockout  L
					INNER JOIN @LockoutValues LV ON L.LockoutId = LV.LockoutId 

			--Update null for Lockout values of Person detail page.
			IF NOT EXISTS(SELECT 1 FROM Lockout WHERE LockoutPageId = 3 AND Lockout = 1)
			BEGIN
				UPDATE Lockout
				SET LockoutDate = NULL
				WHERE LockoutPageId = 3
			END
		 COMMIT TRAN LockoutDetails
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN LockoutDetails

		DECLARE @ErrorMessage NVARCHAR(2000)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

END
