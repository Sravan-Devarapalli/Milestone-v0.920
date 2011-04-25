CREATE PROCEDURE [dbo].[GetCompanyLogoData]
AS
BEGIN

	SELECT TOP(1)  Title,FileName,FilePath,Data
	FROM  CompanyLogoSetting

END

