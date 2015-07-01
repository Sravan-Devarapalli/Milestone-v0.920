-- =============================================
-- Description:	Get BusinessDevelopmentProject.
-- Updated by:	ThulasiRam.P
-- Update date:	04-12-2012
-- =============================================
CREATE PROCEDURE [dbo].[GetBusinessDevelopmentProject]
AS
BEGIN
	SELECT  P.ProjectId,
			P.Name,
			P.ProjectNumber,
			P.IsNoteRequired
	FROM [dbo].[Project] AS P
	WHERE (ProjectNumber IN ('P999918') OR P.IsBusinessDevelopment = 1)
END

