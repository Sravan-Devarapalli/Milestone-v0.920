ALTER TABLE [Skills].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [CKPersonTraining_StartDate] CHECK  (([StartDate]>='1970-1-1' AND [StartDate]<=getdate()))
GO
ALTER TABLE [Skills].[PersonTraining] CHECK CONSTRAINT [CKPersonTraining_StartDate]
