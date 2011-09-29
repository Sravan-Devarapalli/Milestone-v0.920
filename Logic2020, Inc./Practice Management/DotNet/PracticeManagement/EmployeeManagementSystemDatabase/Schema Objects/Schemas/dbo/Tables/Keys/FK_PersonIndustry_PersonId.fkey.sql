ALTER TABLE [dbo].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_PersonId]
GO
