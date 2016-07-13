CREATE PROCEDURE [dbo].[SaveVendorAttachment]
(
	@Id			  INT,
    @FileName			  NVARCHAR(256),	
	@AttachmentData	      VARBINARY(MAX),
	@UploadedDate         DATETIME ,
	@UserLogin            NVARCHAR(255)
)
AS
	SET NOCOUNT ON
	BEGIN
	
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @PersonId INT
	SELECT @PersonId = PersonId FROM dbo.Person P WHERE P.alias = @UserLogin

	  INSERT INTO VendorAttachment
			   ([VendorId]
			   ,[FileName]
			   ,[AttachmentData]
			   ,UploadedDate
			   ,ModifiedBy
			   )     
		 VALUES
			   (@Id
			   ,@FileName
			   ,@AttachmentData
			   ,@UploadedDate
			   ,@PersonId
			   )
      
      EXEC dbo.SessionLogUnprepare
	END
