CREATE PROCEDURE dbo.DeleteProjectAttachmentByProjectId
(
	@ProjectId		INT
)
AS
BEGIN

    IF EXISTS(SELECT 1 FROM ProjectAttachment WHERE ProjectId = @ProjectId)
    BEGIN
		DELETE
		FROM [dbo].ProjectAttachment
		WHERE ProjectId = @ProjectId
	END

END
