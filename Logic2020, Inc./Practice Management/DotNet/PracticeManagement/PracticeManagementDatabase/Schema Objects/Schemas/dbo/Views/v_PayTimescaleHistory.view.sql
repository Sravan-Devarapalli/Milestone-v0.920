CREATE VIEW [dbo].[v_PayTimescaleHistory]
AS 
	WITH PAYWthRank
	AS
	(
	 SELECT p.Person AS PersonId,
			CONVERT(DATE,p.StartDate) StartDate,
			CONVERT(DATE,p.EndDate) EndDate  ,
			p.Timescale
	   FROM dbo.Pay AS p   
	),
	PersonPayGroupStartDates
	AS
	(
	SELECT p1.PersonId,p1.Timescale,p1.StartDate,ROW_NUMBER () OVER (partition by p1.PersonId ORDER BY P1.startdate) as PayRank
	FROM PAYWthRank p1
	LEFT JOIN PAYWthRank p2 ON p1.PersonId = p2.PersonId AND p1.Timescale = p2.Timescale AND p1.StartDate= p2.EndDate 
	WHERE p2.PersonId IS  NULL
	),
	PersonPayGroupEndDates
	AS
	(
	SELECT p1.PersonId,p1.Timescale,p1.EndDate,ROW_NUMBER () OVER (partition by p1.PersonId ORDER BY P1.startdate) as PayRank
	FROM PAYWthRank p1
	LEFT JOIN PAYWthRank p2 ON p1.PersonId = p2.PersonId AND p1.Timescale = p2.Timescale AND p1.EndDate = p2.StartDate
	WHERE p2.PersonId IS NULL
	)

	SELECT ps.PersonId,ps.Timescale,ps.Startdate,DATEADD(dd,-1,pe.Enddate) AS Enddate
	FROM PersonPayGroupStartDates ps
	INNER JOIN PersonPayGroupEndDates pe ON ps.PersonId = pe.PersonId AND ps.PayRank = pe.PayRank AND ps.Timescale = pe.Timescale
