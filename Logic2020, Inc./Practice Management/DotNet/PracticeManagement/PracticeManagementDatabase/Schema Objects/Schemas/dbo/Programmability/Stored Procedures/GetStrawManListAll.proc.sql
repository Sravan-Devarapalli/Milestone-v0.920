CREATE PROCEDURE [dbo].[GetStrawManListAll]
AS
BEGIN
	DECLARE @FutureDate DATETIME
	SELECT @FutureDate = dbo.GetFutureDate()

	SELECT	P.PersonId,
			P.LastName,
			P.FirstName,
			p.PersonStatusId,
			pay.Person AS PayPersonId,
			ps.Name AS PersonStatusName,
			pay.Amount,
			pay.Timescale,
			ts.Name AS TimescaleName,
			pay.StartDate,
			pay.EndDate,
			pay.VacationDays,
			CASE WHEN	(
							SELECT COUNT(*) FROM dbo.MilestonePerson mp 
							INNER JOIN dbo.MilestonePersonEntry mpe ON mp.MilestonePersonId = mpe.MilestonePersonId 
							WHERE mp.PersonId = p.PersonId
						) > 0  
						OR 
						(
							SELECT COUNT (*)
							FROM [dbo].[OpportunityPersons] OP
							WHERE OP.PersonId = p.PersonId
						) > 0
				THEN 1 
				ELSE 0  End  AS InUse
	FROM dbo.Person P
	INNER JOIN dbo.PersonStatus ps
	ON p.PersonStatusId = ps.PersonStatusId
	LEFT JOIN dbo.Pay pay
	ON p.PersonId = pay.Person AND pay.EndDate = @FutureDate
	LEFT JOIN dbo.Timescale ts
	ON pay.Timescale = ts.TimescaleId 
	WHERE P.IsStrawman = 1
	ORDER BY P.LastName,P.FirstName

END

