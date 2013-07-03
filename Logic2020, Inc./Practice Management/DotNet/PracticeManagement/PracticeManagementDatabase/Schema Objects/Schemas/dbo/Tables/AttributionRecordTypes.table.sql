CREATE TABLE [dbo].[AttributionRecordTypes]
(
	[AttributionRecordId] INT  NOT NULL,
	[Name]				  NVARCHAR(100) NOT NULL,
	[IsPercentageType]	  BIT NOT NULL,
	[IsRangeType]		  BIT NOT NULL
)

