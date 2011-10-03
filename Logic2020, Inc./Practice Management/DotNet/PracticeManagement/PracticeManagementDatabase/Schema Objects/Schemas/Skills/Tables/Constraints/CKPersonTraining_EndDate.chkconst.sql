ALTER TABLE [Skills].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [CKPersonTraining_EndDate] CHECK  (([EndDate]>='1970-1-1' AND [EndDate]<=getdate()))
GO
ALTER TABLE [Skills].[PersonTraining] CHECK CONSTRAINT [CKPersonTraining_EndDate]
GO
