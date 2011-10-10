ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_PersonId]
