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
			PA.CategoryId,
			P.PersonId,
			P.LastName,
			P.FirstName
	FROM dbo.ProjectAttachment PA
	LEFT JOIN dbo.Person P ON PA.ModifiedBy = P.PersonId
	WHERE ProjectId = @ProjectId
END
