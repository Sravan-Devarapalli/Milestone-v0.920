ALTER TABLE [Skills].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [FK_PersonTraining_TrainingTypeId] FOREIGN KEY([TenantId],[TrainingTypeId])
REFERENCES [Skills].[TrainingType] ([TenantId],[TrainingTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonTraining] CHECK CONSTRAINT [FK_PersonTraining_TrainingTypeId]
