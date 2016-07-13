CREATE PROCEDURE [dbo].[GetVendorsListWithFilters]
(
	@ShowActive BIT = 0,
	@showInactive BIT = 0,
	@VendorTypesList NVARCHAR(MAX) = NULL,
	@Looked NVARCHAR(40)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @Looked IS NOT NULL
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	ELSE
		SET @Looked = '%'

	DECLARE @VendorTypeList TABLE (Id INT)
	INSERT INTO @VendorTypeList
	SELECT * FROM dbo.ConvertStringListIntoTable(@VendorTypesList)

	SELECT v.Id,
		   v.Name,
		   v.ContactName,
		   v.Email,
		   v.TelephoneNumber,
		   v.Status,
		   vt.Id as VendorTypeId,
		   vt.Name as VendorTypeName
	FROM dbo.Vendor v
	JOIN dbo.VendorType vt on v.VendorTypeId=vt.Id
	WHERE (@VendorTypesList IS NULL OR v.VendorTypeId IN (SELECT * FROM @VendorTypeList))
		  AND ((@ShowActive=1 AND v.Status=1)
				OR (@showInactive=1 AND v.Status=0)
		      )
		  AND(v.ContactName LIKE @Looked OR v.Email LIKE @Looked OR v.Name LIKE @Looked)

END
