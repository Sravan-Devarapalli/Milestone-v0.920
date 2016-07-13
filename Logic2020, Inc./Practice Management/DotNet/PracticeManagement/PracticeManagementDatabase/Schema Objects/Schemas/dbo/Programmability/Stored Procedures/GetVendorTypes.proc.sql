CREATE PROCEDURE [dbo].[GetVendorTypes]
AS
BEGIN
	SELECT v.Id,
		   v.Name
		   FROM dbo.VendorType v
		   ORDER BY v.Name
END 
