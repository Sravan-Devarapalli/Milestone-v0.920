ALTER TABLE [Skills].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_IndustryId] FOREIGN KEY([IndustryId])
REFERENCES [Skills].[Industry] ([IndustryId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_IndustryId]
