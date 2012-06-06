CREATE PROCEDURE [dbo].[SaveProjectAttachment]
(
	@ProjectId			  INT,
	@CategoryId			  INT, 
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

	DECLARE @PersonId INT
	SELECT @PersonId = PersonId FROM dbo.Person P WHERE P.alias = @UserLogin

	  INSERT INTO ProjectAttachment
			   ([ProjectId]
			   ,[CategoryId]
			   ,[FileName]
			   ,[AttachmentData]
			   ,UploadedDate
			   ,ModifiedBy
			   )     
		 VALUES
			   (@ProjectId
			   ,@CategoryId
			   ,@FileName
			   ,@AttachmentData
			   ,@UploadedDate
			   ,@PersonId
			   )
      
      EXEC dbo.SessionLogUnprepare
	END
