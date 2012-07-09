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
                AND ( P.HireDate - ( DATEPART(dw, P.HireDate) - 1 ) ) <= @StartDate
                AND ( @StartDate < ISNULL(PSH.EndDate, @FutureDate)
                        AND @EndDate > PSH.StartDate
                    )
        ORDER BY p.LastName ,
                p.FirstName

    END

