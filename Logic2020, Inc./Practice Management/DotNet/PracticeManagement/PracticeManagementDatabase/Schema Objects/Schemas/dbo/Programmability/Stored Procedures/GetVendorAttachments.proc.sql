CREATE PROCEDURE [dbo].[GetVendorAttachments]
(
	@Id INT
)
AS
BEGIN
	SELECT  VA.Id,
			VA.FileName,
			DATALENGTH(VA.AttachmentData) AS AttachmentSize,
			VA.UploadedDate,
			P.PersonId,
			P.LastName,
			P.FirstName
	FROM dbo.VendorAttachment VA
	LEFT JOIN dbo.Person P ON VA.ModifiedBy = P.PersonId
	WHERE VendorId = @Id
END
