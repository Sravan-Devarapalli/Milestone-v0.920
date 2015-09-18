CREATE PROCEDURE [dbo].[GetSalaryPayTypes]
AS
BEGIN

	SELECT	TimescaleId,
			Name AS Timescale,
			TimescaleCode
	FROM dbo.Timescale
	WHERE	IsContractType = 0

	UNION ALL

	SELECT -1,'1099-Hourly','1099'

END
