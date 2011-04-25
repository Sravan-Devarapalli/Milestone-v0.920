--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-11-2008
-- Description:	Retrieves the overhead rate type
-- =============================================
CREATE PROCEDURE [dbo].[OverheadRateTypeGetById]
(
	@OverheadRateTypeId   INT
)
AS
	SET NOCOUNT ON
	
	SELECT t.[OverheadRateTypeId],
	       t.[Name],
	       t.[HoursToCollect],
	       t.[IsPercentage]
	  FROM dbo.[OverheadRateType] AS t
	 WHERE t.[OverheadRateTypeId] = @OverheadRateTypeId

