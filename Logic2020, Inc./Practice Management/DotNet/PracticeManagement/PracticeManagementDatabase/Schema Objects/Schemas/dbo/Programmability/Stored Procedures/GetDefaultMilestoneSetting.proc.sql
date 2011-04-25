CREATE PROCEDURE [dbo].[GetDefaultMilestoneSetting]

AS
	SELECT TOP 1 
			ClientId,
			ProjectId,
			MilestoneId,
			ModifiedDate,
			LowerBound,
			UpperBound
	FROM [DefaultMileStoneSetting]
	ORDER BY ModifiedDate DESC

