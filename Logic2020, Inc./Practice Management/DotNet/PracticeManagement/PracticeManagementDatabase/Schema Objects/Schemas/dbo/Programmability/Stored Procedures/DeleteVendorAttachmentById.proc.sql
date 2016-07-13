CREATE PROCEDURE [dbo].[DeleteVendorAttachmentById]
(
	@AttachmentId	INT,
	@VendorId		INT,
	@UserLogin      NVARCHAR(255)
)
AS
BEGIN

    IF EXISTS(SELECT 1 FROM VendorAttachment WHERE VendorId = @VendorId)
    BEGIN
    EXEC SessionLogPrepare @UserLogin = @UserLogin
    
		DELETE
		FROM [dbo].VendorAttachment
		WHERE VendorId = @VendorId
			AND (Id = @AttachmentId OR @AttachmentId IS NULL)
		
	EXEC dbo.SessionLogUnprepare
	END

END
