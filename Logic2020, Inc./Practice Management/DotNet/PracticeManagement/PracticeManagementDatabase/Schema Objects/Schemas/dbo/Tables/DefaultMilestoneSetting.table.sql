CREATE TABLE [dbo].[DefaultMilestoneSetting]
(
	ClientId		INT,
	ProjectId		INT,
	MilestoneId		INT,
	ModifiedDate	DATETIME,
	DefaultMilestoneSettingId INT IDENTITY(1,1),
	LowerBound		INT,
	UpperBound		INT
)

