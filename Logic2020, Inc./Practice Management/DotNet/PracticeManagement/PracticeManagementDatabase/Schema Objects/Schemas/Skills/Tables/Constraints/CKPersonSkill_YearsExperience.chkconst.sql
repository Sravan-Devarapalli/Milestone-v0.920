ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [CKPersonSkill_YearsExperience] CHECK  (([YearsExperience]>=(1) AND [YearsExperience]<=(40)))
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [CKPersonSkill_YearsExperience]
GO
