CREATE FUNCTION [dbo].[GetProjectCapabilitiesNames]
(
	@ProjectId	INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @rep NVARCHAR(MAX)
	SET @rep = ''

	SELECT @rep= @rep + CONVERT(NVARCHAR(255),ISNULL(P.Abbreviation,P.Name)) +' - '+ CONVERT(NVARCHAR(255),C.CapabilityName) + ';'
	FROM dbo.ProjectCapabilities PC
	JOIN dbo.PracticeCapabilities C ON C.CapabilityId = PC.CapabilityId
	JOIN dbo.Practice P ON P.PracticeId = C.PracticeId
    WHERE PC.ProjectId = @ProjectId
	ORDER BY ISNULL(P.Abbreviation,P.Name),C.CapabilityName

	RETURN @rep
END
