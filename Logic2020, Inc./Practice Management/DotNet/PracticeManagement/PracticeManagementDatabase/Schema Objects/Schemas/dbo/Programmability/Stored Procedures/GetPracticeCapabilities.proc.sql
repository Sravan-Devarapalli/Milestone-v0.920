CREATE PROCEDURE [dbo].[GetPracticeCapabilities]
(
	@PracticeId INT = NULL,
	@CapabilityId INT = NULL
)
AS
BEGIN
	SELECT	CapabilityId,
			PC.[CapabilityName] AS Name,
			PC.PracticeId,
			ISNULL(P.Abbreviation,P.Name)  AS PracticeAbbreviation,
			PC.IsActive
	FROM dbo.[PracticeCapabilities] PC
	INNER JOIN dbo.Practice  P ON P.PracticeId = PC.PracticeId 
	WHERE	( @PracticeId IS NULL OR P.PracticeId = @PracticeId ) 
			AND ( @CapabilityId IS NULL OR PC.CapabilityId = @CapabilityId ) 
	ORDER BY ISNULL(P.Abbreviation,P.Name),PC.[CapabilityName]
END

