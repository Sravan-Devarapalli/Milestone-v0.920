ALTER TABLE [dbo].[PersonPractice]  WITH CHECK ADD  CONSTRAINT [FK_PersonPractice_PracticeId] FOREIGN KEY([PracticeId])
REFERENCES [dbo].[Practice] ([PracticeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonPractice] CHECK CONSTRAINT [FK_PersonPractice_PracticeId]
