ALTER TABLE [dbo].[PersonContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_PersonContactInfo_ContactInfoType] FOREIGN KEY([ContactInfoTypeId])
REFERENCES [dbo].[ContactInfoType] ([ContactInfoTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonContactInfo] CHECK CONSTRAINT [FK_PersonContactInfo_ContactInfoType]
