ALTER TABLE [Skills].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [FK_PersonTraining_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonTraining] CHECK CONSTRAINT [FK_PersonTraining_PersonId]
