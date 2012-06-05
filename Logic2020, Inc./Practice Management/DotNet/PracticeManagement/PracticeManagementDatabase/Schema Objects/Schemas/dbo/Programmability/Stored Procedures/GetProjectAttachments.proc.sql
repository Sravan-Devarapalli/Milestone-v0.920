CREATE PROCEDURE GetProjectAttachments
(
	@ProjectId INT
)
AS
BEGIN
	SELECT Id,
			FileName,
			DATALENGTH(AttachmentData) AS AttachmentSize,
			UploadedDate,
			PA.CategoryId
	FROM dbo.ProjectAttachment PA
	WHERE ProjectId = @ProjectId
END
