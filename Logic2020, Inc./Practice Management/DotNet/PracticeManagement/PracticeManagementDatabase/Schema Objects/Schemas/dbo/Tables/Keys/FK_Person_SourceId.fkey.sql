ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_SourceId] 
	FOREIGN KEY (SourceId)
	REFERENCES [RecruitingMetrics] ([RecruitingMetricsId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


