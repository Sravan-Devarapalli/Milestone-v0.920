CREATE PROCEDURE [dbo].[TimeScaleGetAll]
AS
	SET NOCOUNT ON

	SELECT t.TimescaleId, t.Name, t.DefaultTerms
	FROM dbo.Timescale AS t
