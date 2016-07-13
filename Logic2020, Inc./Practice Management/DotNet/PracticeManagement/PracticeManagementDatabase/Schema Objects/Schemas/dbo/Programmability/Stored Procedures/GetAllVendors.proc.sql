CREATE PROCEDURE [dbo].[GetAllActiveVendors]
AS
BEGIN

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
	WHERE v.Status=1
END
