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
	FirstName NVARCHAR(40),
	LastName NVARCHAR(40),
	TerminationDate DATETIME,
	Alias NVARCHAR(100),
	DefaultPractice INT,
	IsTerminatedDueToPay BIT,
	ReHiredate DATETIME,
	TimeScaleName NVARCHAR(50),
	TitleName NVARCHAR(100),
	TelephoneNumber NCHAR(100),
	SeniorityName NVARCHAR(50),
	IsOffshore			BIT,
	ManagerId	INT,
	ManagerName	NVARCHAR(100),
	DivisionId			INT
	)
	
	INSERT INTO @TerminatedPersons
	SELECT	PersonId,
			FirstName,
			LastName,
			TerminationDate,
			Alias,
			DefaultPractice,
			0 AS IsTerminatedDueToPay,
			NULL AS ReHiredate,
			NULL AS TimeScaleName,
			NULL AS TitleName,
			NULL AS TelephoneNumber,
			NULL AS SeniorityName,
			null,null,null,null
	FROM dbo.Person AS P
	WHERE   P.PersonStatusId <> 2
			AND P.TerminationDate = (@Today-1)


	INSERT INTO @TerminatedPersons
	SELECT	p.PersonId,
			p.FirstName,
			p.LastName,
			(@Today - 1),
			P.Alias,
			P.DefaultPractice,
			1 AS IsTerminatedDueToPay,
			MIN(Apay.StartDate) AS ReHiredate,
			AT.Name AS TimeScaleName,
			TT.Title AS TitleName,
			P.TelephoneNumber,
			S.Name AS SeniorityName,
			p.IsOffshore,
			p.ManagerId,
			manager.LastName+', '+manager.FirstName AS Manager,
			P.DivisionId
	FROM dbo.Person P  
	INNER JOIN dbo.Pay Bpay  ON Bpay.Person = P.PersonId AND (P.TerminationDate IS NULL OR P.TerminationDate > (@Today-1))  AND Bpay.EndDate = @Today
	INNER JOIN dbo.Timescale BT ON BT.TimescaleId = Bpay.Timescale AND BT.IsContractType = 1 
	INNER JOIN dbo.Pay Apay  ON Apay.Person = P.PersonId AND Apay.StartDate >= @Today 
	INNER JOIN dbo.Timescale AT ON AT.TimescaleId = Apay.Timescale AND AT.IsContractType = 0
	INNER JOIN dbo.Title TT ON TT.TitleId = P.TitleId
	INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
	LEFT JOIN dbo.Person manager ON manager.PersonId = P.ManagerId
	GROUP BY p.PersonId,
			p.FirstName,
			p.LastName,
			p.IsOffshore,
			p.ManagerId,
			P.Alias,
			P.DefaultPractice,
			manager.LastName,
			AT.Name,
			manager.FirstName,
			P.DivisionId,
			P.TelephoneNumber,
			TT.Title,
			S.Name

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
	SET P.ManagerId = PD.DivisionOwnerId
	FROM dbo.Person P
	INNER JOIN @TerminatedPersons AS Manager ON P.ManagerId = Manager.PersonId AND (Manager.IsTerminatedDueToPay = 0 OR ReHiredate > @Today)
	INNER JOIN dbo.PersonDivision AS PD ON Pd.DivisionId=p.DivisionId

	--Lock out Terminated persons
	UPDATE m
	SET m.IsLockedOut = 1,
		m.LastLockoutDate = (@Today - 1)	
	FROM    dbo.aspnet_Users AS u
	INNER JOIN dbo.aspnet_Applications AS a ON u.ApplicationId = a.ApplicationId
	INNER JOIN  dbo.aspnet_Membership AS m ON u.UserId = m.UserId
	INNER JOIN @TerminatedPersons as P ON u.LoweredUserName = LOWER(P.Alias)
	WHERE a.LoweredApplicationName =LOWER('PracticeManagement') --AND P.IsTerminatedDueToPay = 0


	-- set Terminated status to persons
	UPDATE P
	SET P.PersonStatusId = 2, --Terminated status
		P.TerminationDate = (@Today-1),
		P.IsWelcomeEmailSent = 0,
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

	SELECT 
		FirstName,
		LastName,
		TerminationDate,
		IsTerminatedDueToPay,
		ReHiredate,
		Alias,
		TelephoneNumber,
		TimeScaleName,
		TitleName,	
		IsOffshore,	
		ManagerId,
		ManagerName,
		DivisionId,
		CASE WHEN EXISTS(SELECT 1 FROM  @TerminatedPersons AS P 
									 INNER JOIN aspnet_Users AS a ON LOWER(p.Alias) = a.LoweredUserName
									 INNER JOIN aspnet_UsersInRoles AS ur ON a.UserId = ur.UserId
									 INNER JOIN aspnet_Roles AS r ON ur.RoleId = r.RoleId WHERE r.RoleName = 'System Administrator') THEN 1
            ELSE 0 END As isAdministrator,
		SeniorityName
	FROM @TerminatedPersons 
	 
END

