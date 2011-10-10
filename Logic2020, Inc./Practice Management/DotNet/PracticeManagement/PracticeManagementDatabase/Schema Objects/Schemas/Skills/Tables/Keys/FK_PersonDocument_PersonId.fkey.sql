ALTER TABLE [Skills].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [Skills].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_PersonId]
GO
