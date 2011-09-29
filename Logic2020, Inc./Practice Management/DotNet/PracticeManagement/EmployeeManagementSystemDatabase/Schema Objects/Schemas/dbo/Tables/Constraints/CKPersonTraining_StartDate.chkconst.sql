ALTER TABLE [dbo].[PersonTraining]  WITH CHECK ADD  CONSTRAINT [CKPersonTraining_StartDate] CHECK  (([StartDate]>='1970-1-1' AND [StartDate]<=getdate()))
GO
ALTER TABLE [dbo].[PersonTraining] CHECK CONSTRAINT [CKPersonTraining_StartDate]
