--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-3-2008
-- Updated by:	
-- Update date: 
-- Description:	Retrives a timescale by its ID.
-- =============================================
CREATE PROCEDURE [dbo].[TimescaleGetById]
(
	@TimescaleId   INT
)
AS
	SET NOCOUNT ON

	SELECT t.TimescaleId, t.Name, t.DefaultTerms
	  FROM dbo.Timescale AS t
	 WHERE t.TimescaleId = @TimescaleId

