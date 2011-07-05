CREATE PROCEDURE dbo.SaveProjectAttachment
(
	@ProjectId			  INT, 
    @FileName			  NVARCHAR(256),	
	@AttachmentData	      VARBINARY(MAX),
	@UploadedDate         DATETIME 
)
AS
	SET NOCOUNT ON
	BEGIN
	IF (
	    NOT EXISTS(SELECT 1 
				   FROM [dbo].ProjectAttachment 
				   WHERE [ProjectId] = @ProjectId 
				   )
	    )
	  BEGIN
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
      END
      ELSE
      BEGIN
		UPDATE ProjectAttachment
		SET [FileName] = @FileName			
			,AttachmentData =  @AttachmentData
			,UploadedDate   = @UploadedDate
		WHERE ProjectId = @ProjectId
      END
	END
