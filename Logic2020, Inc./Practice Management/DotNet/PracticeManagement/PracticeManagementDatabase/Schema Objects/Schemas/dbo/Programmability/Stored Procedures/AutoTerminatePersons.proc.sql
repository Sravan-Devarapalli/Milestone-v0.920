CREATE PROCEDURE dbo.AutoTerminatePersons
AS
BEGIN
	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))
	-- Close a current compensation for the terminated persons
	
	DECLARE @TerminatedPersons TABLE
	(
	PersonID INT,
	TerminationDate DATETIME,
	Alias nvarchar(100),
	DefaultPractice INT
	)
	
	INSERT INTO @TerminatedPersons
	SELECT	PersonId,
			TerminationDate,
			Alias,
			DefaultPractice
	FROM dbo.Person
	WHERE TerminationDate <= @Today 
			AND PersonStatusId <> 2
			AND TerminationDate >= @Today-1 --Taking one days back from today
	UPDATE Pay
	   SET EndDate = P.TerminationDate
	FROM dbo.Pay 
	JOIN @TerminatedPersons P ON pay.Person = P.PersonId
	WHERE Pay.EndDate > P.TerminationDate
			AND Pay.StartDate < P.TerminationDate

	DELETE dbo.Pay FROM dbo.Pay
	JOIN @TerminatedPersons P ON dbo.Pay.Person = P.PersonId
	WHERE  dbo.Pay.StartDate >= P.TerminationDate
	 
	-- SET new manager for subordinates

	UPDATE P
	SET P.ManagerId = COALESCE(SamePracticedManagerOrUp.PersonId,DefalutManager.PersonId,pr.PracticeManagerId)
	FROM dbo.Person P
	JOIN @TerminatedPersons Manager ON P.ManagerId = Manager.PersonId
	LEFT JOIN Practice pr ON P.DefaultPractice = pr.PracticeId
	OUTER APPLY(
				SELECT TOP 1 PersonId
				FROM dbo.Person P1
				WHERE P1.DefaultPractice = Manager.DefaultPractice
						AND P.PersonId <> P1.PersonId
						AND P1.SeniorityId <= 65
						AND P1.PersonStatusId = 1
						AND P1.PersonId <> Manager.PersonID
						AND (p1.TerminationDate > @Today OR p1.TerminationDate IS NULL)
				ORDER BY ISNULL(p1.TerminationDate,dbo.GetFutureDate()) DESC
					) SamePracticedManagerOrUp
	OUTER APPLY(
				SELECT TOP 1 PersonId
				FROM dbo.Person P2
				WHERE P2.IsDefaultManager = 1
						AND P.PersonId <> P2.PersonId
						AND P2.PersonId <> manager.PersonID
						AND P2.PersonStatusId = 1
						AND (p2.TerminationDate > @Today OR p2.TerminationDate IS NULL)
				ORDER BY ISNULL(p2.TerminationDate,dbo.GetFutureDate()) DESC
				) DefalutManager

	--Lock out Terminated persons
	UPDATE m
	SET IsLockedOut = 1,
		LastLockoutDate = @Today
	FROM    dbo.aspnet_Users AS u
	JOIN dbo.aspnet_Applications AS a ON u.ApplicationId = a.ApplicationId
	JOIN  dbo.aspnet_Membership AS m ON u.UserId = m.UserId
	JOIN @TerminatedPersons as P ON u.LoweredUserName = LOWER(P.Alias) 
	WHERE a.LoweredApplicationName =LOWER('PracticeManagement')

	-- set Terminated status to persons
	UPDATE P
	SET PersonStatusId = 2 --Terminated status
	FROM dbo.Person P 
	JOIN @TerminatedPersons TP ON P.PersonId = TP.PersonID

	--Update Person Status History
	UPDATE PH
	 SET EndDate = @Today-1
	 FROM dbo.PersonStatusHistory PH
	 JOIN @TerminatedPersons P ON P.PersonId = PH.PersonID
	 WHERE EndDate IS NULL 
			AND StartDate != @Today
	 
	INSERT INTO [dbo].[PersonStatusHistory]
			   ([PersonId]
			   ,[PersonStatusId]
			   ,[StartDate]
			   )
	SELECT PersonID,
		   2, --Terminated
		   @Today
	FROM @TerminatedPersons
END

