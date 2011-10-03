ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [CKPersonSkill_LastUsed] CHECK  (([LastUsed]>=(1970) AND [LastUsed]<=datepart(year,getdate())))
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [CKPersonSkill_LastUsed]
GO
