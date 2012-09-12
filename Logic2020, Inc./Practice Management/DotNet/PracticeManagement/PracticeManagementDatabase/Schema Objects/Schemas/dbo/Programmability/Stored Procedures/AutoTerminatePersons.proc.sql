CREATE PROCEDURE dbo.AutoTerminatePersons
AS
BEGIN

	DECLARE @Today DATETIME,
			@FutureDate DATETIME,
			@TerminationReasonId INT

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE()))),
		   @FutureDate = dbo.GetFutureDate()
	SELECT @TerminationReasonId = TR.TerminationReasonId FROM dbo.TerminationReasons TR WHERE TR.TerminationReason = 'Voluntary - 1099 Contract Ended'
	-- Close a current compensation for the terminated persons
	
	DECLARE @TerminatedPersons TABLE
	(
	PersonID INT,
	TerminationDate DATETIME,
	Alias NVARCHAR(100),
	DefaultPractice INT,
	IsTerminatedDueToPay BIT,
	ReHiredate DATETIME
	)
	
	INSERT INTO @TerminatedPersons
	SELECT	PersonId,
			TerminationDate,
			Alias,
			DefaultPractice,
			0 AS IsTerminatedDueToPay,
			NULL AS ReHiredate 
	FROM dbo.Person AS P
	WHERE   P.PersonStatusId <> 2
			AND P.TerminationDate = (@Today-1)


	INSERT INTO @TerminatedPersons
	SELECT	p.PersonId,
			@Today,
			P.Alias,
			P.DefaultPractice,
			1 AS IsTerminatedDueToPay,
			MIN(Apay.StartDate) AS ReHiredate 
	FROM dbo.Person P  
	INNER JOIN dbo.Pay Bpay  ON Bpay.Person = P.PersonId AND (P.TerminationDate IS NULL OR P.TerminationDate > (@Today-1))  AND Bpay.EndDate = @Today
	INNER JOIN dbo.Timescale BT ON BT.TimescaleId = Bpay.Timescale AND BT.IsContractType = 1 
	INNER JOIN dbo.Pay Apay  ON Apay.Person = P.PersonId AND Apay.StartDate >= @Today 
	INNER JOIN dbo.Timescale AT ON AT.TimescaleId = Apay.Timescale AND AT.IsContractType = 0
	GROUP BY p.PersonId,
			P.Alias,
			P.DefaultPractice

	UPDATE pay
	   SET pay.EndDate = P.TerminationDate + 1
	FROM dbo.Pay AS pay
	INNER JOIN @TerminatedPersons P ON pay.Person = P.PersonID AND P.IsTerminatedDueToPay = 0
	WHERE pay.EndDate > P.TerminationDate + 1
			AND pay.StartDate < P.TerminationDate + 1

	DELETE pay
	FROM dbo.Pay AS pay
	INNER JOIN @TerminatedPersons P ON pay.Person = P.PersonId AND P.IsTerminatedDueToPay = 0
	WHERE  pay.StartDate >= P.TerminationDate + 1
	 
	-- SET new manager for subordinates

	UPDATE P
	SET P.ManagerId = COALESCE(SamePracticedManagerOrUp.PersonId,DefalutManager.PersonId,pr.PracticeManagerId)
	FROM dbo.Person P
	INNER JOIN @TerminatedPersons AS Manager ON P.ManagerId = Manager.PersonId AND (Manager.IsTerminatedDueToPay = 0 OR ReHiredate > @Today)
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
	WHERE a.LoweredApplicationName =LOWER('PracticeManagement') AND P.IsTerminatedDueToPay = 0


	-- set Terminated status to persons
	UPDATE P
	SET P.PersonStatusId = 2, --Terminated status
		P.TerminationDate = (@Today-1),
		P.TerminationReasonId = CASE WHEN TP.IsTerminatedDueToPay = 0 THEN P.TerminationReasonId ELSE @TerminationReasonId END
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
	FROM @TerminatedPersons TP

	--Rehire Person
	UPDATE P
	SET P.PersonStatusId = 1, --ACTIVE status
		P.HireDate = TP.ReHiredate,
		P.TerminationDate = NULL,
		P.TerminationReasonId = NULL
	FROM dbo.Person AS P 
	INNER JOIN @TerminatedPersons TP ON P.PersonId = TP.PersonID AND TP.IsTerminatedDueToPay = 1

	--Update Person Status History
	UPDATE PH
	 SET PersonStatusId = 1
	 FROM dbo.PersonStatusHistory PH
	 INNER JOIN @TerminatedPersons P ON P.PersonId = PH.PersonID AND P.IsTerminatedDueToPay = 1
	 WHERE EndDate IS NULL AND StartDate = @Today
	 
END

