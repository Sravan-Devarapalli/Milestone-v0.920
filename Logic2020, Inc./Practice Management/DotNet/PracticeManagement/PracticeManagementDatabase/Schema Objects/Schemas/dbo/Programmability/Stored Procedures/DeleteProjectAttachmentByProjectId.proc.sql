CREATE PROCEDURE [dbo].[DeleteProjectAttachmentByProjectId]
(
	@AttachmentId	INT,
	@ProjectId		INT,
	@UserLogin      NVARCHAR(255)
)
AS
BEGIN

    IF EXISTS(SELECT 1 FROM ProjectAttachment WHERE ProjectId = @ProjectId)
    BEGIN
    EXEC SessionLogPrepare @UserLogin = @UserLogin
    
		DELETE
		FROM [dbo].ProjectAttachment
		WHERE ProjectId = @ProjectId
			AND (Id = @AttachmentId OR @AttachmentId IS NULL)
		
	EXEC dbo.SessionLogUnprepare
	END

END
