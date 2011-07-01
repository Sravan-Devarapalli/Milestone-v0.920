CREATE PROCEDURE [dbo].[SaveMarginInfoDefaults]
(
	@GoalTypeId   INT,
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
	DELETE dbo.DefaultMarginColorInfo 
	WHERE GoalTypeId = @GoalTypeId
	END
	
	INSERT INTO [dbo].DefaultMarginColorInfo
           (GoalTypeId
           ,[ColorId]
           ,[StartRange]
           ,[EndRange])
     VALUES
           (@GoalTypeId
           ,@ColorId
           ,@StartRange
           ,@EndRange)
	
END
