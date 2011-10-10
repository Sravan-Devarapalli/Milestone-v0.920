ALTER TABLE [Skills].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [Skills].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_PersonId]
GO
