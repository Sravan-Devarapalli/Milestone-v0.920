CREATE TRIGGER [tr_PracticeCapabilities_Log]
    ON [dbo].PracticeCapabilities
   AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
	-- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	
	

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END
