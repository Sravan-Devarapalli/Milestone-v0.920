CREATE TRIGGER [tr_PersonDelete]
ON [dbo].[Person]
INSTEAD OF DELETE
AS
BEGIN

	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL

	--Deleting the pay history
	DELETE Pa
	FROM dbo.Pay Pa
	INNER JOIN dbo.Person P ON Pa.Person = P.PersonId
	INNER JOIN deleted d ON P.PersonId = d.PersonId

	--Deleting the Status History
	DELETE PSH
	FROM dbo.PersonStatusHistory PSH
	INNER JOIN deleted d ON PSH.PersonId = d.PersonId

	--Deleting the Person Calendar Auto Records
	DELETE PCA
	FROM dbo.PersonCalendarAuto PCA
	INNER JOIN deleted d ON PCA.PersonId = d.PersonId

	--Deleting the person As We are using "INSTEAD OF DELETE" we need to perform delete again
	DELETE P
	FROM dbo.Person P
	INNER JOIN deleted d ON P.PersonId = d.PersonId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END



