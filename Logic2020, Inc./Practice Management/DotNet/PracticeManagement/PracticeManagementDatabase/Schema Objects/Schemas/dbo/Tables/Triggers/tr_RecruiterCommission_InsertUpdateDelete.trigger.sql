CREATE TRIGGER [tr_RecruiterCommission_InsertUpdateDelete]
    ON [dbo].[RecruiterCommission]
    FOR INSERT, UPDATE, DELETE 
    AS 
    BEGIN
    	SET NOCOUNT ON;

		-- Ensure the temporary table exists
		EXEC SessionLogPrepare @UserLogin = NULL
		
		DECLARE @insertTime DATETIME
		SELECT @insertTime = dbo.InsertingTime()

		INSERT INTO PersonHistory([PersonId]
								  ,[HireDate]
								  ,[TerminationDate]
								  ,[Alias]
								  ,[DefaultPractice]
								  ,[FirstName]
								  ,[LastName]
								  ,[Notes]
								  ,[PersonStatusId]
								  ,[EmployeeNumber]
								  ,[SeniorityId]
								  ,[ManagerId]
								  ,[PracticeOwnedId]
								  ,[IsDefaultManager]
								  ,[TelephoneNumber]
								  ,[IsWelcomeEmailSent]
								  ,[MiddleName]
								  ,[ImageUrl]
								  ,[IsStrawman]
								  ,[IsOffshore]
								  ,[PaychexID]
								  ,[DivisionId]
								  ,[TerminationReasonId]
								  ,[RecruiterId]
								  ,[CreatedDate]
								  ,[CreatedBy])
		SELECT P.[PersonId]
			  ,P.[HireDate]
			  ,P.[TerminationDate]
			  ,P.[Alias]
			  ,P.[DefaultPractice]
			  ,P.[FirstName]
			  ,P.[LastName]
			  ,P.[Notes]
			  ,P.[PersonStatusId]
			  ,P.[EmployeeNumber]
			  ,P.[SeniorityId]
			  ,P.[ManagerId]
			  ,P.[PracticeOwnedId]
			  ,P.[IsDefaultManager]
			  ,P.[TelephoneNumber]
			  ,P.[IsWelcomeEmailSent]
			  ,P.[MiddleName]
			  ,P.[ImageUrl]
			  ,P.[IsStrawman]
			  ,P.[IsOffshore]
			  ,P.[PaychexID]
			  ,P.[DivisionId]
			  ,P.[TerminationReasonId]
			  ,i.[RecruiterId]
			  ,@insertTime
			  ,l.PersonID
		FROM inserted i
		INNER JOIN dbo.Person P ON P.PersonId = i.RecruitId
		INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		LEFT JOIN deleted d ON d.RecruitId = i.RecruitId
		WHERE i.RecruiterId <> ISNULL(d.RecruiterId, 0)

		INSERT INTO PersonHistory([PersonId]
								  ,[HireDate]
								  ,[TerminationDate]
								  ,[Alias]
								  ,[DefaultPractice]
								  ,[FirstName]
								  ,[LastName]
								  ,[Notes]
								  ,[PersonStatusId]
								  ,[EmployeeNumber]
								  ,[SeniorityId]
								  ,[ManagerId]
								  ,[PracticeOwnedId]
								  ,[IsDefaultManager]
								  ,[TelephoneNumber]
								  ,[IsWelcomeEmailSent]
								  ,[MiddleName]
								  ,[ImageUrl]
								  ,[IsStrawman]
								  ,[IsOffshore]
								  ,[PaychexID]
								  ,[DivisionId]
								  ,[TerminationReasonId]
								  ,[RecruiterId]
								  ,[CreatedDate]
								  ,[CreatedBy])
		SELECT P.[PersonId]
			  ,P.[HireDate]
			  ,P.[TerminationDate]
			  ,P.[Alias]
			  ,P.[DefaultPractice]
			  ,P.[FirstName]
			  ,P.[LastName]
			  ,P.[Notes]
			  ,P.[PersonStatusId]
			  ,P.[EmployeeNumber]
			  ,P.[SeniorityId]
			  ,P.[ManagerId]
			  ,P.[PracticeOwnedId]
			  ,P.[IsDefaultManager]
			  ,P.[TelephoneNumber]
			  ,P.[IsWelcomeEmailSent]
			  ,P.[MiddleName]
			  ,P.[ImageUrl]
			  ,P.[IsStrawman]
			  ,P.[IsOffshore]
			  ,P.[PaychexID]
			  ,P.[DivisionId]
			  ,P.[TerminationReasonId]
			  ,R.RecruiterId
			  ,@insertTime
			  ,l.PersonID
		FROM deleted d
		INNER JOIN dbo.Person P ON P.PersonId = d.RecruitId
		INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
		OUTER APPLY (SELECT TOP 1 * FROM RecruiterCommission RC WITH(NOLOCK) WHERE RC.RecruitId = d.RecruitId) R
		LEFT JOIN inserted i ON d.RecruitId = i.RecruitId
		WHERE i.RecruitId IS NULL AND R.RecruitId IS NULL
		

		-- End logging session
		EXEC dbo.SessionLogUnprepare
    END
