ALTER TABLE [Skills].[PersonContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonContactInfo_ContactInfoType] FOREIGN KEY([TenantId],[ContactInfoTypeId])
REFERENCES [Skills].[ContactInfoType] ([TenantId],[ContactInfoTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonContactInfo] CHECK CONSTRAINT [FK_PersonContactInfo_ContactInfoType]
