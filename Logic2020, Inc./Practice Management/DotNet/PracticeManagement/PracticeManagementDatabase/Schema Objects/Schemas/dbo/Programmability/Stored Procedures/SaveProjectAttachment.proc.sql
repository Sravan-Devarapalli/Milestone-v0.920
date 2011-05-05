CREATE PROCEDURE dbo.SaveProjectAttachment
(
	@ProjectId			  INT, 
    @FileName			  NVARCHAR(256),	
	@AttachmentData	      VARBINARY(MAX)
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
			   ,[AttachmentData])     
		 VALUES
			   (@ProjectId
			   ,@FileName			   
			   ,@AttachmentData)
      END
      ELSE
      BEGIN
		UPDATE ProjectAttachment
		SET [FileName] = @FileName			
			,AttachmentData =  @AttachmentData
		WHERE ProjectId = @ProjectId
      END
	END
