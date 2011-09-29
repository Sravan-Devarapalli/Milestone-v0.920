ALTER TABLE [dbo].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [CKPersonEmployer_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [dbo].[PersonEmployer] CHECK CONSTRAINT [CKPersonEmployer_DisplayOrder]

