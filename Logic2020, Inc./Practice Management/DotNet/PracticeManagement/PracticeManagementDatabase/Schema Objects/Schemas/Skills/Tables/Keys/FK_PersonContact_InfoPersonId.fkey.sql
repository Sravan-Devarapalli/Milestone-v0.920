ALTER TABLE [Skills].[PersonContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonContact_InfoPersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [Skills].[PersonContactInfo] CHECK CONSTRAINT [FK_PersonContact_InfoPersonId]
GO
