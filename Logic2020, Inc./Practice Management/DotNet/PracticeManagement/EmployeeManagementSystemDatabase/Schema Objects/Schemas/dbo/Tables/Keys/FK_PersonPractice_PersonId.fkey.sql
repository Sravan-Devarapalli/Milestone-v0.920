ALTER TABLE [dbo].[PersonPractice]  WITH CHECK ADD  CONSTRAINT [FK_PersonPractice_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonPractice] CHECK CONSTRAINT [FK_PersonPractice_PersonId]

