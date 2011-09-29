ALTER TABLE [dbo].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [CKPersonEmployer_EndDate] CHECK  (([EndDate]>='1970-1-1' AND [EndDate]<=dateadd(month,(3),getdate())))
GO
ALTER TABLE [dbo].[PersonEmployer] CHECK CONSTRAINT [CKPersonEmployer_EndDate]
GO
