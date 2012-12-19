CREATE PROCEDURE [dbo].[GetStrawmanDetailsByIdWithCurrentPay]
(
	@StrawmanId INT
)
AS
BEGIN

	DECLARE @HoursPerYear DECIMAL, @FutureDate DATETIME
	SELECT @HoursPerYear = dbo.GetHoursPerYear(), @FutureDate = dbo.GetFutureDate()

	SELECT	PersonId,
			LastName,
			FirstName,
			p.PersonStatusId,
			pay.Person AS PayPersonId,
			ps.Name AS PersonStatusName,
			pay.Amount,
			pay.Timescale,
			ts.Name AS TimescaleName,
			pay.StartDate,
			pay.EndDate,
			pay.VacationDays,
			pay.BonusAmount,
			pay.BonusHoursToCollect,
			CAST(CASE pay.BonusHoursToCollect WHEN @HoursPerYear THEN 1 ELSE 0 END AS BIT) AS IsYearBonus,
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
	FROM dbo.Person p
	LEFT JOIN dbo.PersonStatus ps
		ON p.PersonStatusId = ps.PersonStatusId
	LEFT JOIN dbo.Pay pay
		ON p.PersonId = pay.Person AND pay.EndDate = @FutureDate
	LEFT JOIN dbo.Timescale ts
		ON pay.Timescale = ts.TimescaleId 
	WHERE IsStrawman = 1 AND p.personId = @StrawmanId

END
