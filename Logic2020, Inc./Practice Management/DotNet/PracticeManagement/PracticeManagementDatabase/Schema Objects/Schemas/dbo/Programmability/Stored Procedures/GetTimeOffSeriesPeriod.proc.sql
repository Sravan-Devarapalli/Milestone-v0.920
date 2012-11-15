CREATE PROCEDURE [dbo].[GetTimeOffSeriesPeriod]
(
	@PersonId INT, 
	@Date	DATETIME
)
AS
BEGIN

DECLARE @SeriesID BIGINT 

SELECT @SeriesID = PC.SeriesId 
FROM  dbo.PersonCalendar AS PC
WHERE PC.Date = @Date AND PC.PersonId = @PersonId

        SELECT MIN(PC.Date) 'StartDate',  MAX(PC.Date) 'EndDate', 
		PC.ApprovedBy 'ApprovedBy', p.FirstName + ', ' + p.LastName 'ApprovedByName'
		FROM dbo.PersonCalendar AS PC
		INNER JOIN dbo.Person AS P ON P.PersonId = Pc.PersonId
		WHERE PC.SeriesId = @SeriesID
		GROUP BY Pc.SeriesId, p.FirstName , p.LastName ,PC.ApprovedBy

END

