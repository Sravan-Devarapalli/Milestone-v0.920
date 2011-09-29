ALTER TABLE [dbo].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_PersonId] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_PersonId]
