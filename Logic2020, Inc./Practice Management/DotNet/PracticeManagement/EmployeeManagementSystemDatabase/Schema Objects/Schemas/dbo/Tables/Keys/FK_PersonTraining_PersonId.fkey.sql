ALTER TABLE [dbo].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [FK_PersonTraining_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonTraining] CHECK CONSTRAINT [FK_PersonTraining_PersonId]
