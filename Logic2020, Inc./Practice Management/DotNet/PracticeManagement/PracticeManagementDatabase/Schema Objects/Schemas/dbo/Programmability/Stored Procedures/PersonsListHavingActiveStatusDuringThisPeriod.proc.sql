CREATE PROCEDURE [dbo].[PersonsListHavingActiveStatusDuringThisPeriod]
(
	@StartDate     DATETIME ,
	@EndDate       DATETIME 
)
AS
BEGIN
		SELECT p.PersonId,
		p.FirstName,
		p.LastName,
		p.IsDefaultManager
		FROM dbo.Person AS p
		INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId 
		WHERE 
		p.IsStrawman = 0
		AND PSH.PersonStatusId = 1 -- ACTIVE Status
		AND ( 
			 @StartDate BETWEEN PSH.StartDate  AND ISNULL(PSH.EndDate,dbo.GetFutureDate()) OR
			 @EndDate BETWEEN  PSH.StartDate  AND ISNULL(PSH.EndDate,dbo.GetFutureDate())
			)
		ORDER BY p.LastName, p.FirstName

END
