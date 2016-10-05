CREATE PROCEDURE [dbo].[GetProjectsForClients]
(
	@ClientIds NVARCHAR(MAX) = NULL	
)
AS 
BEGIN

		DECLARE @ClientIdsTable TABLE ( Id INT )
		INSERT  INTO @ClientIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ClientIds)

		SELECT	vp.ProjectId,
				vp.ProjectNumber+'-'+vp.Name as 'Name'
		FROM v_Project vp
		WHERE (@ClientIds IS NULL OR vp.ClientId IN (select Id FROM @ClientIdsTable))

END
