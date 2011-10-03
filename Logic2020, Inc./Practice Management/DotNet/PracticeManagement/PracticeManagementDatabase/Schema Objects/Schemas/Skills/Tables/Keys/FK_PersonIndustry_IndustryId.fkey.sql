ALTER TABLE [Skills].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_IndustryId] FOREIGN KEY([TenantId],[IndustryId])
REFERENCES [Skills].[Industry] ([TenantId],[IndustryId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_IndustryId]
