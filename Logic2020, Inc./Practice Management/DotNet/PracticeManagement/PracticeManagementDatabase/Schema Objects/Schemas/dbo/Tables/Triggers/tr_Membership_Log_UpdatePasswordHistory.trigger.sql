CREATE TRIGGER [dbo].[tr_Membership_Log_UpdatePasswordHistory]
   ON  [dbo].[aspnet_Membership]
   AFTER  UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	  
	  DECLARE @OldPasswordChangedDate DATETIME ,@NewPasswordChangedDate DATETIME 
	  
	 SELECT  @OldPasswordChangedDate = d.LastPasswordChangedDate 
	 FROM deleted AS d
	  
	 SELECT  @NewPasswordChangedDate = i.LastPasswordChangedDate 
		FROM inserted AS i
		
    IF(@OldPasswordChangedDate != @NewPasswordChangedDate)	
    BEGIN								
	  INSERT INTO UserPassWordHistory([UserId]
	   ,[Password]
	   ,[PasswordFormat]
	   ,[PasswordSalt]
	   ,[LastPasswordChangedDate])
	   SELECT d.UserId,d.Password,d.PasswordFormat,d.PasswordSalt,d.LastPasswordChangedDate    
	   FROM  deleted AS d 
	END
END

