-- ==========================================================================================
-- Author:		Sainath CH
-- Create date: 03-16-2012
-- Description: Get Person List with the search keyword.
-- Updated by : Sainath.CH
-- Update Date: 03-30-2012
-- ==========================================================================================
CREATE PROCEDURE [dbo].[GetPersonListBySearchKeyword]
(
	@Looked		   NVARCHAR(40) = NULL
)
AS
BEGIN
	
		DECLARE @NOW DATETIME ,@FutureDate DATETIME
		SELECT @NOW = dbo.GettingPMTime(GETUTCDATE()),@FutureDate = dbo.GetFutureDate()

		IF @Looked IS NOT NULL
			SET @Looked = '%' + LTRIM(RTRIM(@Looked)) + '%'
		ELSE
			SET @Looked = '%'

		SELECT P.PersonId,
			P.LastName,
			P.FirstName,
			PS.Name AS PersonStatusName,
			TS.Name AS Timescale
		FROM dbo.Person P
		INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = P.PersonStatusId AND P.IsStrawman = 0 
		LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,@FutureDate) 
		LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
		WHERE  P.FirstName LIKE @Looked OR P.LastName LIKE @Looked
		ORDER BY P.LastName,P.FirstName

END
