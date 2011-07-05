CREATE PROCEDURE [dbo].[UpdateLastPasswordChangedDateForPerson]
@Email NVARCHAR(256)
AS 
BEGIN
	UPDATE dbo.aspnet_Membership 
	SET  LastPasswordChangedDate = CreateDate
	WHERE  Email = @Email
END 
GO
