-- =============================================
-- Author:		ThulasiRam.P
-- Modified date: 2012-02-14
-- Description:	Update existing time type
-- =============================================
CREATE PROCEDURE dbo.TimeTypeUpdate
(
	@TimeTypeId INT,
	@Name VARCHAR(50),
	@IsDefault BIT,
	@IsActive	BIT,
	@IsInternal BIT,
	@IsAdministrative BIT
)
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
		
	UPDATE dbo.TimeType
	SET [Name] = @Name,
		[IsActive] = @IsActive,
		[IsAdministrative] = @IsAdministrative
	WHERE TimeTypeId = @TimeTypeId
END

