CREATE PROCEDURE [dbo].[PersonsListHavingActiveStatusDuringThisPeriod]
    (
      @StartDate DATETIME ,
      @EndDate DATETIME 
    )
AS 
    BEGIN
		DECLARE @FutureDate DATETIME
		SELECT @FutureDate = dbo.GetFutureDate()

        SELECT DISTINCT p.PersonId ,
                p.FirstName ,
                p.LastName ,
                p.IsDefaultManager
        FROM    dbo.Person AS p
                INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId
        WHERE   p.IsStrawman = 0
                AND PSH.PersonStatusId = 1 -- ACTIVE Status
                AND P.HireDate  <= @EndDate AND ISNULL(P.TerminationDate,@FutureDate)  >= @StartDate 
				AND PSH.StartDate <= @EndDate AND ISNULL(PSH.EndDate,@FutureDate)  >= @StartDate
        ORDER BY p.LastName ,
                 p.FirstName

    END

