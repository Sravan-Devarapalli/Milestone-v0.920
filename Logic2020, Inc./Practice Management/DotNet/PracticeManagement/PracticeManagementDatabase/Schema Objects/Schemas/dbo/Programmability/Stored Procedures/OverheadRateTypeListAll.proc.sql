--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-27-2008
-- Description:	Retrieves the list the overhead rate types.
-- =============================================
CREATE PROCEDURE [dbo].[OverheadRateTypeListAll]
AS
	SET NOCOUNT ON

	SELECT t.OverheadRateTypeId,
	       t.Name,
	       t.IsPercentage,
	       t.HoursToCollect
	  FROM dbo.OverheadRateType AS t
	ORDER BY t.HoursToCollect, t.Name

