ALTER TABLE [Skills].[PersonPractice]  WITH CHECK ADD  CONSTRAINT [FK_PersonPractice_PersonId] FOREIGN KEY([TenantId],[PersonId])
REFERENCES [Skills].[Person] ([TenantId],[PersonId])
GO
ALTER TABLE [Skills].[PersonPractice] CHECK CONSTRAINT [FK_PersonPractice_PersonId]

