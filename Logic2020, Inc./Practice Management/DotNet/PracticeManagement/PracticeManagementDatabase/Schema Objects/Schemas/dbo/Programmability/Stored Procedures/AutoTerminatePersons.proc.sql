CREATE PROCEDURE dbo.AutoTerminatePersons
AS
BEGIN

	DECLARE @Today DATETIME,
			@FutureDate DATETIME

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE()))),
		   @FutureDate = dbo.GetFutureDate()
	-- Close a current compensation for the terminated persons
	
	DECLARE @TerminatedPersons TABLE
	(
	PersonID INT,
	TerminationDate DATETIME,
	Alias NVARCHAR(100),
	DefaultPractice INT
	)
	
	INSERT INTO @TerminatedPersons
	SELECT	PersonId,
			TerminationDate,
			Alias,
			DefaultPractice
	FROM dbo.Person AS P
	WHERE   P.PersonStatusId <> 2
			AND P.TerminationDate = (@Today-1)

	UPDATE pay
	   SET pay.EndDate = P.TerminationDate + 1
	FROM dbo.Pay AS pay
	INNER JOIN @TerminatedPersons P ON pay.Person = P.PersonID
	WHERE pay.EndDate > P.TerminationDate + 1
			AND pay.StartDate < P.TerminationDate + 1

	DELETE pay
	FROM dbo.Pay AS pay
	INNER JOIN @TerminatedPersons P ON pay.Person = P.PersonId
	WHERE  pay.StartDate >= P.TerminationDate + 1
	 
	-- SET new manager for subordinates

	UPDATE P
	SET P.ManagerId = COALESCE(SamePracticedManagerOrUp.PersonId,DefalutManager.PersonId,pr.PracticeManagerId)
	FROM dbo.Person P
	INNER JOIN @TerminatedPersons AS Manager ON P.ManagerId = Manager.PersonId
	LEFT  JOIN Practice pr ON P.DefaultPractice = pr.PracticeId
	OUTER APPLY(
				SELECT TOP 1 PersonId
				FROM dbo.Person P1
				JOIN dbo.Seniority S ON P1.SeniorityId = s.SeniorityId
				WHERE P1.DefaultPractice = Manager.DefaultPractice
						AND P.PersonId <> P1.PersonId
						AND S.SeniorityValue <= 65
						AND P1.PersonStatusId IN (1,5)
						AND P1.PersonId <> Manager.PersonID
						AND (p1.TerminationDate > @Today OR p1.TerminationDate IS NULL)
				ORDER BY ISNULL(p1.TerminationDate,@FutureDate) DESC
					) SamePracticedManagerOrUp
	OUTER APPLY(
				SELECT TOP 1 PersonId
				FROM dbo.Person P2
				WHERE P2.IsDefaultManager = 1
						AND P.PersonId <> P2.PersonId
						AND P2.PersonId <> manager.PersonID
						AND P2.PersonStatusId IN (1,5)
						AND (p2.TerminationDate > @Today OR p2.TerminationDate IS NULL)
				ORDER BY ISNULL(p2.TerminationDate,@FutureDate) DESC
				) DefalutManager

	--Lock out Terminated persons
	UPDATE m
	SET m.IsLockedOut = 1,
		m.LastLockoutDate = @Today
	FROM    dbo.aspnet_Users AS u
	INNER JOIN dbo.aspnet_Applications AS a ON u.ApplicationId = a.ApplicationId
	INNER JOIN  dbo.aspnet_Membership AS m ON u.UserId = m.UserId
	INNER JOIN @TerminatedPersons as P ON u.LoweredUserName = LOWER(P.Alias) 
	WHERE a.LoweredApplicationName =LOWER('PracticeManagement')

	-- set Terminated status to persons
	UPDATE P
	SET P.PersonStatusId = 2 --Terminated status
	FROM dbo.Person AS P 
	INNER JOIN @TerminatedPersons TP ON P.PersonId = TP.PersonID

	--Update Person Status History
	UPDATE PH
	 SET EndDate = @Today-1
	 FROM dbo.PersonStatusHistory PH
	 INNER JOIN @TerminatedPersons P ON P.PersonId = PH.PersonID
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

