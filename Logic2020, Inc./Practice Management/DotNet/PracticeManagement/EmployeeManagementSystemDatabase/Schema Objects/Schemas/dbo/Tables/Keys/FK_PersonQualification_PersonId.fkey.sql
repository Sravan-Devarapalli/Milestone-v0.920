ALTER TABLE [dbo].[PersonQualification]  WITH CHECK ADD  CONSTRAINT [FK_PersonQualification_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonQualification] CHECK CONSTRAINT [FK_PersonQualification_PersonId]
GO
