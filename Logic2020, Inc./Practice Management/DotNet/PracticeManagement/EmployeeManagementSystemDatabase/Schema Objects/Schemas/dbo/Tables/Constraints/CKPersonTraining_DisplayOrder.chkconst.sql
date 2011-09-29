ALTER TABLE [dbo].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [CKPersonTraining_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [dbo].[PersonTraining] CHECK CONSTRAINT [CKPersonTraining_DisplayOrder]
GO
