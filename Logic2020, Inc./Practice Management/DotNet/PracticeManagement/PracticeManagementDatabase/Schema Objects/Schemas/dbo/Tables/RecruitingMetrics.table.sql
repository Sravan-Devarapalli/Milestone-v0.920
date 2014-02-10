CREATE TABLE [dbo].[RecruitingMetrics]
(
    [RecruitingMetricsId]		INT			 IDENTITY (1, 1) NOT NULL,
    [Name]						NVARCHAR(50) NOT NULL,
	[RecruitingMetricsTypeId]	INT			 NOT NULL,
	[SortOrder]					INT			 NOT NULL
)

