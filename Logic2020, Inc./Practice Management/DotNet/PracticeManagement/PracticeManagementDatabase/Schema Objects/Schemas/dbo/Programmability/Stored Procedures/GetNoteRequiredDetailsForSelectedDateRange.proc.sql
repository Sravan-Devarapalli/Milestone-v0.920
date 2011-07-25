CREATE PROCEDURE [dbo].[GetNoteRequiredDetailsForSelectedDateRange]
	@StartDate DATETIME, 
	@EndDate DATETIME,
	@PersonId INT
AS
BEGIN
SET NOCOUNT ON;

	;WITH DateList AS (   
				SELECT @StartDate AS 'Date'
				UNION ALL  
				SELECT DATEADD(dd, 1, [Date])  
				FROM DateList s  
				WHERE DATEADD(dd, 1, [Date]) <= @EndDate
				)

		
		SELECT d.Date,COALESCE(P2.IsNotesRequired,P1.IsNotesRequired,1) IsNotesRequired
		FROM DateList as d
		CROSS JOIN  Person as p
		LEFT JOIN Practice as P1 on  P1.PracticeId = p.DefaultPractice
		LEFT JOIN  dbo.Pay  on  d.[Date]  BETWEEN pay.StartDate AND pay.EndDate-1 AND pay.Person = p.PersonId
		LEFT JOIN Practice as P2 on  P2.PracticeId = pay.PracticeId
		WHERE d.Date is not null and P.PersonId = @PersonId

END
	
