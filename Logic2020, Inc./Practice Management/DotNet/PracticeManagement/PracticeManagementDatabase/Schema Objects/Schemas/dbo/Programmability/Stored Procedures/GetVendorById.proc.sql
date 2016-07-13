CREATE PROCEDURE [dbo].[GetVendorById]
(
	@Id	INT
)
AS
	SET NOCOUNT ON

	SELECT v.Id,
		   v.Name,
		   v.ContactName,
		   v.Email,
		   v.Status,
		   v.TelephoneNumber,
		   vt.Id as VendorTypeId,
		   vt.Name as VendorTypeName
	FROM dbo.Vendor v
	JOIN dbo.VendorType vt on v.VendorTypeId=vt.Id
		 WHERE v.Id=@Id
