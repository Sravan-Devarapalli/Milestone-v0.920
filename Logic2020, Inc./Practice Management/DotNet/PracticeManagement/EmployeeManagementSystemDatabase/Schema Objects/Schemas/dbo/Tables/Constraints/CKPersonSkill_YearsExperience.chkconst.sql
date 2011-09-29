ALTER TABLE [dbo].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [CKPersonSkill_YearsExperience] CHECK  (([YearsExperience]>=(1) AND [YearsExperience]<=(40)))
GO
ALTER TABLE [dbo].[PersonSkill] CHECK CONSTRAINT [CKPersonSkill_YearsExperience]
GO
