-- ==========================================================================================
-- Author:		Sainath CH
-- Create date: 03-13-2012
-- Description: Get Person List Filtered by person current statues and current pay types
-- ==========================================================================================
CREATE PROCEDURE [dbo].[GetPersonListByPayTypeIdsAndStatusIds]
(
	@TimescaleIds NVARCHAR(MAX) = NULL,
	@PersonStatusIds NVARCHAR(MAX) = NULL
)
AS
BEGIN
	
		DECLARE @NOW DATETIME 

		SELECT @NOW = dbo.GettingPMTime(GETUTCDATE())

		DECLARE @TimescaleIdList table (Id int)
		INSERT INTO @TimescaleIdList
		SELECT * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)

		DECLARE @PersonStatusIdList table (Id int)
		INSERT INTO @PersonStatusIdList
		SELECT * FROM dbo.ConvertStringListIntoTable(@PersonStatusIds)

		SELECT P.PersonId,
			P.LastName,
			P.FirstName,
			PS.Name AS PersonStatusName,
			TS.Name AS Timescale
		FROM dbo.Person P
		INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = P.PersonStatusId AND P.IsStrawman = 0 
		LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate,dbo.GetFutureDate()) 
		LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
		WHERE (@TimescaleIds IS NULL OR PA.Timescale IN (SELECT ID FROM @TimescaleIdList)) 
				AND ((@PersonStatusIds IS NULL) OR P.PersonStatusId IN (SELECT Id FROM @PersonStatusIdList))
		ORDER BY 	P.LastName,P.FirstName

END
