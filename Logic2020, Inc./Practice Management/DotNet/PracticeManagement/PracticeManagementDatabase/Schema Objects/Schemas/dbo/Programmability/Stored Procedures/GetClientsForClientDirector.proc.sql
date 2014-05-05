CREATE PROCEDURE [dbo].[GetClientsForClientDirector]
(
	@DirectorId	INT=NULL
)
AS
BEGIN
	IF @DirectorId IS NOT NULL
	BEGIN
		SELECT	DISTINCT C.ClientId,
				C.Code AS ClientCode,
				C.Name AS ClientName
		FROM dbo.Client C
		INNER JOIN dbo.Project P ON P.ClientId = C.ClientId
		WHERE P.ProjectStatusId IN (3,4) --Active and Completed status 
		ORDER BY C.Name
	END
	ELSE
	BEGIN
		SELECT	ClientId,
				C.Code AS ClientCode,
				C.Name AS ClientName
		FROM dbo.Client C 
		ORDER BY C.Name
	END
END
