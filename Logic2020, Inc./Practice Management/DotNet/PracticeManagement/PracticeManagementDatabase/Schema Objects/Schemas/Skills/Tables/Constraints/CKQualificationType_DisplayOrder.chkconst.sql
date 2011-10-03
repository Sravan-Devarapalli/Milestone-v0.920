ALTER TABLE [Skills].[QualificationType]  WITH CHECK ADD  CONSTRAINT [CKQualificationType_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(200)))
GO
ALTER TABLE [Skills].[QualificationType] CHECK CONSTRAINT [CKQualificationType_DisplayOrder]
GO
