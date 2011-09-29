ALTER TABLE [dbo].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [FK_PersonEmployer_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonEmployer] CHECK CONSTRAINT [FK_PersonEmployer_PersonId]
GO
