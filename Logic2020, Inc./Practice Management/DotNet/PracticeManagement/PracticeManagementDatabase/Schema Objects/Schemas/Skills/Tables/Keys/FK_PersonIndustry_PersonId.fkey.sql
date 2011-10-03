ALTER TABLE [Skills].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [FK_PersonIndustry_PersonId] FOREIGN KEY([TenantId],[PersonId])
REFERENCES [Skills].[Person] ([TenantId],[PersonId])
GO
ALTER TABLE [Skills].[PersonIndustry] CHECK CONSTRAINT [FK_PersonIndustry_PersonId]
GO
