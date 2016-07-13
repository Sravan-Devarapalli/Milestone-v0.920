CREATE PROCEDURE [dbo].[GetVendorAttachmentData]
(
	@VendorId	         INT,
	@AttachmentId		 INT
)
AS
BEGIN
	SELECT TOP(1) va.AttachmentData 
	FROM dbo.VendorAttachment AS va
	WHERE va.VendorId = @VendorId
		AND va.Id = @AttachmentId
END

