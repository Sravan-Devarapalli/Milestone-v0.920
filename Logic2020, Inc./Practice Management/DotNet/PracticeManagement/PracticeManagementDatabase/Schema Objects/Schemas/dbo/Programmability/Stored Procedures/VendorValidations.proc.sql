CREATE PROCEDURE [dbo].[VendorValidations]
(
	@Name				NVARCHAR (50),			
	@ContactName		NVARCHAR (50),		
	@Email			    NVARCHAR (50),
	@Id					INT=NULL
)
AS
BEGIN	
	BEGIN TRY
		DECLARE @ErrorMessage NVARCHAR(2048)
			
			IF @Name IS NOT NULL AND EXISTS(SELECT 1
											FROM dbo.[Vendor] AS v
											WHERE v.Name = @Name AND  (@Id IS NULL OR v.Id <> @Id))
			BEGIN
			-- Vendor Name uniqueness violation
				SELECT @ErrorMessage = [dbo].[GetErrorMessage](70022)
				RAISERROR (@ErrorMessage, 16, 1)
			END	
			ELSE IF @ContactName IS NOT NULL AND EXISTS(SELECT 1
											FROM dbo.[Vendor] AS v
											WHERE v.ContactName = @ContactName AND  (@Id IS NULL OR v.Id <> @Id))
			BEGIN
			-- Vendor Name uniqueness violation
				SELECT @ErrorMessage = [dbo].[GetErrorMessage](70023)
				RAISERROR (@ErrorMessage, 16, 1)
			END	
			ELSE IF @Email IS NOT NULL AND EXISTS(SELECT 1
											FROM dbo.[Vendor] AS v
											WHERE v.Email = @Email AND  (@Id IS NULL OR v.Id <> @Id))
			BEGIN
			-- Vendor Name uniqueness violation
				SELECT @ErrorMessage = [dbo].[GetErrorMessage](70024)
				RAISERROR (@ErrorMessage, 16, 1)
			END	
	END TRY
	BEGIN CATCH 
		SELECT @ErrorMessage = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 16, 1)
	END CATCH
END
