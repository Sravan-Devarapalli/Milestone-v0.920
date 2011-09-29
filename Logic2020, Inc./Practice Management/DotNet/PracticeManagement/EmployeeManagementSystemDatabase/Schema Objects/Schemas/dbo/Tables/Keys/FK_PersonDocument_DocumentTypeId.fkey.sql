ALTER TABLE [dbo].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_DocumentTypeId] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_DocumentTypeId]
