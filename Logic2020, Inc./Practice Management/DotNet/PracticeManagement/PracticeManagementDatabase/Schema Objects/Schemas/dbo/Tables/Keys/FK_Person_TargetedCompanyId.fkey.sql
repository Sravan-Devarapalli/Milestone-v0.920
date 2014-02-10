ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_TargetedCompanyId] 
	FOREIGN KEY (TargetedCompanyId)
	REFERENCES [RecruitingMetrics] ([RecruitingMetricsId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


