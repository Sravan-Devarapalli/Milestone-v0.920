-- =============================================
-- Author:		Sainath CH
-- Create date: 03-13-2012
-- Description: Get ALL Pay Types
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPayTypes]
AS
BEGIN
	SELECT  t.TimescaleId ,
			t.Name AS Timescale
	FROM dbo.Timescale t
END

