ALTER TABLE [dbo].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_IndustryId] FOREIGN KEY([IndustryId])
REFERENCES [dbo].[Industry] ([IndustryId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_IndustryId]
