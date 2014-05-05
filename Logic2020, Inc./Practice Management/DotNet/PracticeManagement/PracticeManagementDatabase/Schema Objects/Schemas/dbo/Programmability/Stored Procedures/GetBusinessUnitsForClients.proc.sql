CREATE PROCEDURE [dbo].[GetBusinessUnitsForClients]
(
  @AccountIds	NVARCHAR(MAX)
)
AS
BEGIN
	
	DECLARE @AccountIdsTable TABLE ( Ids INT )
	
	INSERT INTO @AccountIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@AccountIds)

	SELECT	PG.GroupId,
			PG.Name AS GroupName,
			PG.Code AS GroupCode,
			C.ClientId,
			C.Name AS ClientName,
			C.Code AS ClientCode
	FROM dbo.ProjectGroup PG
	INNER JOIN dbo.Client C ON C.ClientId = PG.ClientId
	WHERE C.ClientId IN (SELECT Ids FROM @AccountIdsTable)
END
