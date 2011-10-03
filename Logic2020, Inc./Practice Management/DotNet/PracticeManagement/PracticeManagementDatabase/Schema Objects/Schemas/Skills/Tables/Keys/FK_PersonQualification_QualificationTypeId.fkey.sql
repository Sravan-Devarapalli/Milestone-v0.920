ALTER TABLE [Skills].[PersonQualification]  WITH CHECK ADD  CONSTRAINT [FK_PersonQualification_QualificationTypeId] FOREIGN KEY([TenantId],[QualificationTypeId])
REFERENCES [Skills].[QualificationType] ([TenantId],[QualificationTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonQualification] CHECK CONSTRAINT [FK_PersonQualification_QualificationTypeId]
GO
