ALTER TABLE [dbo].[PersonQualification]  WITH CHECK ADD  CONSTRAINT [FK_PersonQualification_QualificationTypeId] FOREIGN KEY([QualificationTypeId])
REFERENCES [dbo].[QualificationType] ([QualificationTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonQualification] CHECK CONSTRAINT [FK_PersonQualification_QualificationTypeId]
GO
