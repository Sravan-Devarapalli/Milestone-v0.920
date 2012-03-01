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
		SELECT @Date 'StartDate', @Date 'EndDate', PC.ApprovedBy 'ApprovedBy', p.LastName + ', ' + p.FirstName 'ApprovedByName'
		FROM PersonCalendar PC
		LEFT JOIN Person P ON P.PersonId = PC.ApprovedBy
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date
	END
	ELSE
	BEGIN
		
		;WITH AfterConsecutiveDates AS
		(
			SELECT PCC.*
			FROM PersonCalendar PCC
			WHERE PCC.Date = @Date AND PCC.PersonId = @PersonId AND PCC.IsSeries = 1
			UNION ALL
			SELECT PC.*
			FROM AfterConsecutiveDates CD
			JOIN PersonCalendar PC ON ((DATEPART(DW, CD.date) = 6 AND PC.date = DATEADD(DD,3, CD.date) )
											OR  PC.date = DATEADD(DD,1, CD.date)
										)
									AND PC.PersonId = CD.PersonId AND PC.IsSeries = 1 AND PC.TimeTypeId = CD.TimeTypeId
									AND PC.ActualHours = CD.ActualHours AND ISNULL(PC.ApprovedBy, 0) = ISNULL(CD.ApprovedBy, 0)
		),
		BeforeConsecutiveDates AS
		(
			SELECT PCC.*
			FROM PersonCalendar PCC
			WHERE PCC.Date = @Date AND PCC.PersonId = @PersonId AND PCC.IsSeries = 1
			UNION ALL
			SELECT PC.*
			FROM BeforeConsecutiveDates CD
			JOIN PersonCalendar PC ON ((DATEPART(DW, CD.date) = 2 AND PC.date = DATEADD(DD, -3, CD.date))
											OR  PC.date = DATEADD(DD, -1, CD.date)
										)
									AND PC.PersonId = CD.PersonId AND PC.IsSeries = 1 AND PC.TimeTypeId = CD.TimeTypeId
									AND PC.ActualHours = CD.ActualHours AND ISNULL(PC.ApprovedBy, 0) = ISNULL(CD.ApprovedBy, 0)
		)

		SELECT MIN(CD.Date) 'StartDate', MAX(CD.Date) 'EndDate', CD.ApprovedBy 'ApprovedBy', p.LastName + ', ' + p.FirstName 'ApprovedByName'
		FROM 
		(
			SELECT * FROM BeforeConsecutiveDates BD
			UNION
			SELECT * FROM AfterConsecutiveDates AD
		) CD
		LEFT JOIN dbo.Person P ON P.PersonId = CD.ApprovedBy
		GROUP BY CD.PersonId, CD.ApprovedBy, P.FirstName, P.LastName

	END

END
