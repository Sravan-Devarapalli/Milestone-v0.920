ALTER TABLE [Skills].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [CKPersonEmployer_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [Skills].[PersonEmployer] CHECK CONSTRAINT [CKPersonEmployer_DisplayOrder]

