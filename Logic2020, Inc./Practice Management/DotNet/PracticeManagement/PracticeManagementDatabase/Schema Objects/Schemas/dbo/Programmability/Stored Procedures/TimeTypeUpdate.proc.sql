-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Update existing time type
-- =============================================
CREATE PROCEDURE dbo.TimeTypeUpdate
	@TimeTypeId INT,
	@Name VARCHAR(50),
	@IsDefault BIT 
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE TimeType
	SET [Name] = @Name
	WHERE TimeTypeId = @TimeTypeId

	-- If IsDefault value has changed, update table	
	IF @IsDefault = 1
	BEGIN
		UPDATE TimeType
		SET IsDefault = 0
		
		UPDATE TimeType
		SET IsDefault = 1
		WHERE TimeTypeId = @TimeTypeId
	END
END

