ALTER TABLE [dbo].[PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonDocument_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonDocument] CHECK CONSTRAINT [FK_PersonDocument_PersonId]
GO
