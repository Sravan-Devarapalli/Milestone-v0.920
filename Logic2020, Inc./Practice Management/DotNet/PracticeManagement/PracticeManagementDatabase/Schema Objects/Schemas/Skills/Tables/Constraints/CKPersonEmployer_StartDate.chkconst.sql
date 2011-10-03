ALTER TABLE [Skills].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [CKPersonEmployer_StartDate] CHECK  (([StartDate]>='1970-1-1' AND [StartDate]<=getdate()))
GO
ALTER TABLE [Skills].[PersonEmployer] CHECK CONSTRAINT [CKPersonEmployer_StartDate]
GO
