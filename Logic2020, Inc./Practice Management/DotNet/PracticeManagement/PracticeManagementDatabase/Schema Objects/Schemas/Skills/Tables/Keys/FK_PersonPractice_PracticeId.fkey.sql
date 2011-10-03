ALTER TABLE [Skills].[PersonPractice]  WITH CHECK ADD  CONSTRAINT [FK_PersonPractice_PracticeId] FOREIGN KEY([TenantId],[PracticeId])
REFERENCES [Skills].[Practice] ([TenantId],[PracticeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonPractice] CHECK CONSTRAINT [FK_PersonPractice_PracticeId]
