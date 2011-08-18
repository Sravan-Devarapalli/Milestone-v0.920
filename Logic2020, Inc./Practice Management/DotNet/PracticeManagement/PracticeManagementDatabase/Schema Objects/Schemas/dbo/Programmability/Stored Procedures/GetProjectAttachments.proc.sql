CREATE PROCEDURE GetProjectAttachments
(
	@ProjectId INT
)
AS
BEGIN
	SELECT Id,
			FileName,
			DATALENGTH(AttachmentData) AS AttachmentSize,
			UploadedDate
	FROM ProjectAttachment
	WHERE ProjectId = @ProjectId
END
