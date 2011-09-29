ALTER TABLE [dbo].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [FK_PersonTraining_TrainingTypeId] FOREIGN KEY([TrainingTypeId])
REFERENCES [dbo].[TrainingType] ([TrainingTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonTraining] CHECK CONSTRAINT [FK_PersonTraining_TrainingTypeId]
