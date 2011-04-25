CREATE FUNCTION [dbo].[GetErrorMessage]
(
	@MessageId	INT
)
RETURNS NVARCHAR(2048)
AS
BEGIN
	DECLARE @MessageText NVARCHAR(2048)

	
	SELECT @MessageText = MessageText 
	FROM [dbo].[ErrorMessage]
	WHERE MessageId = @MessageId

	
	RETURN @MessageText
END
