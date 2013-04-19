CREATE FUNCTION [dbo].[IsBusinessGroupInUse]
(
	@BusinessGroupId INT,
	@Code VARCHAR(6)
)
RETURNS BIT
AS
BEGIN
	IF(@Code = 'BG0001')
	BEGIN
		RETURN 1
	END
	IF EXISTS(SELECT 1 FROM dbo.ProjectGroup WHERE BusinessGroupId = @BusinessGroupId)
	BEGIN
		RETURN 1
	END
		RETURN 0
END

