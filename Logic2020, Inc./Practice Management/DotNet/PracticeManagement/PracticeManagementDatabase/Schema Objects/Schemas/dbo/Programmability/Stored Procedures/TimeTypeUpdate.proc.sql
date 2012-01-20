-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Update existing time type
-- =============================================
CREATE PROCEDURE dbo.TimeTypeUpdate
	@TimeTypeId INT,
	@Name VARCHAR(50),
	@IsDefault BIT,
	@IsActive	BIT,
	@IsInternal BIT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT 1 FROM dbo.TimeType WHERE [Name] = @Name AND TimeTypeId <> @TimeTypeId)
	BEGIN
		DECLARE @Error NVARCHAR(200)
		SET @Error = 'This work type already exists. Please add a different work type.'
		RAISERROR(@Error,16,1)
		RETURN
	END
		
	UPDATE TimeType
	SET [Name] = @Name,
		[IsActive] = @IsActive
	WHERE TimeTypeId = @TimeTypeId
END

