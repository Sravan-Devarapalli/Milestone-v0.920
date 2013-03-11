CREATE PROCEDURE [dbo].[PracticeListAllWithCapabilities]
AS
BEGIN
	SELECT  P.PracticeId, 
			P.Name,
			P.IsActive,
			P.Abbreviation,
			PC.CapabilityId,
			Pc.CapabilityName,
			Pc.IsActive AS CapabilityIsActive,
			CASE 
				WHEN EXISTS(SELECT 1 FROM dbo.ProjectCapabilities c WHERE c.CapabilityId = PC.CapabilityId)
					THEN CAST(1 AS BIT)
				ELSE CAST(0 AS BIT)
			END AS [InUse]
		FROM dbo.Practice P
		LEFT JOIN dbo.PracticeCapabilities PC ON PC.PracticeId = P.PracticeId
		WHERE PC.PracticeId IS NOT NULL OR ( PC.PracticeId IS NULL AND P.IsActive = 1)
		ORDER BY P.Name,Pc.CapabilityName
END

