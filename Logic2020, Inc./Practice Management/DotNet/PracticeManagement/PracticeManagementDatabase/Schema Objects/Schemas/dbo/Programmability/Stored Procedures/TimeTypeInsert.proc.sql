-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Insert new time type
-- =============================================
CREATE PROCEDURE dbo.TimeTypeInsert
	@Name VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM dbo.TimeType WHERE [Name] = @Name)
	BEGIN
		DECLARE @Error NVARCHAR(200)
		SET @Error = 'This time type already exists. Please add a different time type.'
		RAISERROR(@Error,16,1)
		RETURN
	END

	INSERT INTO dbo.TimeType ([Name]) VALUES (@Name)
END

