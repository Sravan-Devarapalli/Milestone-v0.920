CREATE PROCEDURE [dbo].[PersonsListHavingActiveStatusDuringThisPeriod]
    (
      @StartDate DATETIME ,
      @EndDate DATETIME 
    )
AS 
BEGIN
		DECLARE @FutureDate DATETIME,
			    @StartDateLocal	DATETIME,
				@EndDateLocal	DATETIME
		SELECT @FutureDate = dbo.GetFutureDate()
		SET	   @StartDateLocal = @StartDate
		SET	   @EndDateLocal = @EndDate

		SELECT DISTINCT p.PersonId ,
				per.FirstName ,
				per.LastName ,
				per.PreferredFirstName
		FROM    dbo.v_PersonHistory AS p
				INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = p.PersonId
				INNER JOIN dbo.Person per ON per.PersonId = P.PersonId
		WHERE   PSH.PersonStatusId IN (1,5) -- ACTIVE Status
				AND P.HireDate  <= @EndDate AND ISNULL(P.TerminationDate,@FutureDate)  >= @StartDate 
				AND PSH.StartDate <= @EndDate AND ISNULL(PSH.EndDate,@FutureDate)  >= @StartDate
		ORDER BY per.LastName ,
					per.FirstName
		OPTION (RECOMPILE);
END

