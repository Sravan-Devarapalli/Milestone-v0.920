ALTER TABLE [Skills].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_DocumentTypeId] FOREIGN KEY([TenantId],[DocumentTypeId])
REFERENCES [Skills].[DocumentType] ([TenantId],[DocumentTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_DocumentTypeId]
