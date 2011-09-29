ALTER TABLE [dbo].[PersonQualification]  WITH CHECK ADD  CONSTRAINT [CKPersonQualification_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [dbo].[PersonQualification] CHECK CONSTRAINT [CKPersonQualification_DisplayOrder]
GO
