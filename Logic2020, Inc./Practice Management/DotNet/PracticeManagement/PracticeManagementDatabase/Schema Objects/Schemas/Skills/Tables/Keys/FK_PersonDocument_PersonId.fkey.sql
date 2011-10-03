ALTER TABLE [Skills].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_PersonId] FOREIGN KEY([TenantId],[PersonId])
REFERENCES [Skills].[Person] ([TenantId],[PersonId])
GO
ALTER TABLE [Skills].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_PersonId]
GO
