CREATE PROCEDURE [dbo].[SaveProjectAttachment]
(
	@ProjectId			  INT, 
    @FileName			  NVARCHAR(256),	
	@AttachmentData	      VARBINARY(MAX),
	@UploadedDate         DATETIME ,
	@UserLogin            NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	BEGIN
	
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	  INSERT INTO ProjectAttachment
			   ([ProjectId]
			   ,[FileName]
			   ,[AttachmentData]
			   ,UploadedDate
			   )     
		 VALUES
			   (@ProjectId
			   ,@FileName			   
			   ,@AttachmentData
			   ,@UploadedDate)
      
      EXEC dbo.SessionLogUnprepare
	END
