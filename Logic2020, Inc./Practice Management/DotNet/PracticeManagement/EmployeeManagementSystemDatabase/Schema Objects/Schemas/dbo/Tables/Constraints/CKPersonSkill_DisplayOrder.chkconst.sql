ALTER TABLE [dbo].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [CKPersonSkill_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [dbo].[PersonSkill] CHECK CONSTRAINT [CKPersonSkill_DisplayOrder]
GO
