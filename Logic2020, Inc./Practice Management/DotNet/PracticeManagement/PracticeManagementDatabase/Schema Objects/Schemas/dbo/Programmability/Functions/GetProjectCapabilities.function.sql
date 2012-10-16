CREATE FUNCTION [dbo].[GetProjectCapabilities]
(
	@ProjectId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @rep NVARCHAR(MAX)
	SET @rep = ''

	SELECT @rep= @rep + CONVERT(NVARCHAR(255),PC.CapabilityId) + ','
	FROM dbo.ProjectCapabilities PC
    WHERE PC.ProjectId = @ProjectId

	RETURN @rep
END
