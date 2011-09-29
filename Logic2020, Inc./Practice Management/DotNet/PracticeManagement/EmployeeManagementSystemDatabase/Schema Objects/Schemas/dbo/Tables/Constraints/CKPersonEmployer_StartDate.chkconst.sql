ALTER TABLE [dbo].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [CKPersonEmployer_StartDate] CHECK  (([StartDate]>='1970-1-1' AND [StartDate]<=getdate()))
GO
ALTER TABLE [dbo].[PersonEmployer] CHECK CONSTRAINT [CKPersonEmployer_StartDate]
GO
