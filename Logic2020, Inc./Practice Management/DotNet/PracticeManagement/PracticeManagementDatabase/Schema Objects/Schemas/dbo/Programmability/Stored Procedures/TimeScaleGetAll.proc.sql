CREATE PROCEDURE [dbo].[TimeScaleGetAll]
AS
	SET NOCOUNT ON

	SELECT t.TimescaleId, t.Name
	FROM dbo.Timescale AS t
