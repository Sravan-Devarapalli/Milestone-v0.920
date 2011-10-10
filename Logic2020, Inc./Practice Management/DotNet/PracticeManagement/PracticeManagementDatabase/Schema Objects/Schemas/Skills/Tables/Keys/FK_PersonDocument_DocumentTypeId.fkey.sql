ALTER TABLE [Skills].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_DocumentTypeId] FOREIGN KEY([DocumentTypeId])
REFERENCES [Skills].[DocumentType] ([DocumentTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_DocumentTypeId]
