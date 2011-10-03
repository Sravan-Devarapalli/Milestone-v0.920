ALTER TABLE [Skills].[PersonQualification]  WITH CHECK ADD  CONSTRAINT [CKPersonQualification_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [Skills].[PersonQualification] CHECK CONSTRAINT [CKPersonQualification_DisplayOrder]
GO
