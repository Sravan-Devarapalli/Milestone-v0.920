CREATE PROCEDURE [dbo].[GetTimeOffSeriesPeriod]
(
	@PersonId INT, 
	@Date	DATETIME
)
AS
BEGIN
	
	IF 0 = (SELECT PC.IsSeries
			FROM dbo.PersonCalendar PC
			WHERE PC.PersonId = @PersonId AND PC.Date = @Date)
	BEGIN
		SELECT @Date 'StartDate', @Date 'EndDate'
	END
	ELSE
	BEGIN
		
		;WITH ConsecutiveDates AS
		(
			SELECT PCC.*
			FROM PersonCalendar PCC
			WHERE PCC.Date = @Date AND PCC.PersonId = @PersonId AND PCC.IsSeries = 1
			UNION ALL
			SELECT PC.*
			FROM ConsecutiveDates CD
			JOIN PersonCalendar PC ON ((DATEPART(DW, CD.date) = 6 AND PC.date = DATEADD(DD,3, CD.date) )
											OR (DATEPART(DW, CD.date) = 2 AND PC.date = DATEADD(DD, -3, CD.date))
											OR  PC.date = DATEADD(DD,1, CD.date)
											OR  PC.date = DATEADD(DD, -1, CD.date)
										)
									AND PC.PersonId = CD.PersonId AND PC.IsSeries = 1 AND PC.TimeTypeId = CD.TimeTypeId
									AND PC.ActualHours = CD.ActualHours
		)

		SELECT MIN(CD.Date) 'StartDate', MAX(CD.Date) 'EndDate'
		FROM ConsecutiveDates CD
		GROUP BY CD.PersonId

	END

END
