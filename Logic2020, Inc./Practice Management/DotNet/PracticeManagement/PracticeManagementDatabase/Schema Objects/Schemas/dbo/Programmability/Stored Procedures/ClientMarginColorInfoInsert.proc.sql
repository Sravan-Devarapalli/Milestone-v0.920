CREATE PROCEDURE dbo.ClientMarginColorInfoInsert
(
	@ClientId   INT,
	@ColorId    INT,
	@StartRange INT,
	@EndRange   INT,
	@isDeletePreviousMarginInfo BIT
)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF(@isDeletePreviousMarginInfo=1)
	BEGIN
	DELETE dbo.ClientMarginColorInfo 
	WHERE ClientId = @ClientId
	END
	
	INSERT INTO [dbo].[ClientMarginColorInfo]
           ([ClientId]
           ,[ColorId]
           ,[StartRange]
           ,[EndRange])
     VALUES
           (@ClientId
           ,@ColorId
           ,@StartRange
           ,@EndRange)
	
END
