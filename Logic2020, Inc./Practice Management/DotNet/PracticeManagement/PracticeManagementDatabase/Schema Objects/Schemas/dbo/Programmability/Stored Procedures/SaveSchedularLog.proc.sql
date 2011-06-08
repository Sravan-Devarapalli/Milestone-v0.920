CREATE PROCEDURE dbo.SaveSchedularLog
(
	@LastRun	DATETIME,
	@Status		NVARCHAR(255),
	@Comments	NVARCHAR(4000),
	@NextRun	DATETIME
)
AS
BEGIN
	INSERT INTO [dbo].[SchedularLog]
           ([LastRun]
           ,[Status]
           ,[Comments]
           ,[NextRun]
           )
     VALUES
           (@LastRun
           ,@Status
           ,@Comments
           ,@NextRun)
END
