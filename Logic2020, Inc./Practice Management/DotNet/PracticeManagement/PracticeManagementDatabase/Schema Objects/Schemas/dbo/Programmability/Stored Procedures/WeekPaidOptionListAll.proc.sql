-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-24-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrieves the list of the Week Paid Options
-- =============================================
CREATE PROCEDURE [dbo].[WeekPaidOptionListAll]
AS
	SET NOCOUNT ON

	SELECT WeekPaidOptionId, Name
	  FROM dbo.WeekPaidOption
	ORDER BY WeekPaidOptionId

