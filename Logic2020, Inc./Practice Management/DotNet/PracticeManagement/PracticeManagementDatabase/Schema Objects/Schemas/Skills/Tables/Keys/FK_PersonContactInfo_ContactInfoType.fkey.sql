ALTER TABLE [Skills].[PersonContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonContactInfo_ContactInfoType] FOREIGN KEY([ContactInfoTypeId])
REFERENCES [Skills].[ContactInfoType] ([ContactInfoTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonContactInfo] CHECK CONSTRAINT [FK_PersonContactInfo_ContactInfoType]
