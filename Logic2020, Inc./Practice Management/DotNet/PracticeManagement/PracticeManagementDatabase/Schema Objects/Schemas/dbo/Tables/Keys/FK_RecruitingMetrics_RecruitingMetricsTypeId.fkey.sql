ALTER TABLE [dbo].[RecruitingMetrics]
	ADD CONSTRAINT [FK_RecruitingMetrics_RecruitingMetricsTypeId] 
	FOREIGN KEY ([RecruitingMetricsTypeId])
	REFERENCES [RecruitingMetricsType] (RecruitingMetricsTypeId) ON DELETE NO ACTION ON UPDATE NO ACTION;


