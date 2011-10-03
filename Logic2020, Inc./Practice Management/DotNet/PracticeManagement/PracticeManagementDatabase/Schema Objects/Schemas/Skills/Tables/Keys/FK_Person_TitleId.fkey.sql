ALTER TABLE [Skills].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_TitleId] FOREIGN KEY([TenantId],[TitleId])
REFERENCES [Skills].[Title] ([TenantId],[TitleId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[Person] CHECK CONSTRAINT [FK_Person_TitleId]
GO
