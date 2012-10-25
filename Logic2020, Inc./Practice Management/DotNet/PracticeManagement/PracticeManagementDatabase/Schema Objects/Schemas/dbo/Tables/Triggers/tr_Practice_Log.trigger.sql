CREATE TRIGGER [tr_Practice_Log]
 ON  [dbo].[Practice]
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END
