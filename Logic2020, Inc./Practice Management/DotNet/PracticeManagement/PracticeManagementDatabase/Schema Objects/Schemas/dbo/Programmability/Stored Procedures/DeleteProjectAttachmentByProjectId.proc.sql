CREATE PROCEDURE [dbo].[DeleteProjectAttachmentByProjectId]
(
	@ProjectId		INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN

    IF EXISTS(SELECT 1 FROM ProjectAttachment WHERE ProjectId = @ProjectId)
    BEGIN
    EXEC SessionLogPrepare @UserLogin = @UserLogin
    
		DELETE
		FROM [dbo].ProjectAttachment
		WHERE ProjectId = @ProjectId
		
	EXEC dbo.SessionLogUnprepare
	END

END
