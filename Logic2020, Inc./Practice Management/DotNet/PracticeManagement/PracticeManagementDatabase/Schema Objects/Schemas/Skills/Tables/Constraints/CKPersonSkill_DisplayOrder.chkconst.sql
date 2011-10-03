ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [CKPersonSkill_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [CKPersonSkill_DisplayOrder]
GO
