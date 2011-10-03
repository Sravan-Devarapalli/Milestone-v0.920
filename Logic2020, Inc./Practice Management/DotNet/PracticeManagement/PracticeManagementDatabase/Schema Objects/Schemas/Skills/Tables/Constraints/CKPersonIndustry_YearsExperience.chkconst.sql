ALTER TABLE [Skills].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [CKPersonIndustry_YearsExperience] CHECK  (([YearsExperience]>=(1) AND [YearsExperience]<=(40)))
GO
ALTER TABLE [Skills].[PersonIndustry] CHECK CONSTRAINT [CKPersonIndustry_YearsExperience]
GO
