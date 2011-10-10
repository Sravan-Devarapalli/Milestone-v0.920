ALTER TABLE [Skills].[PersonSkill] ADD  CONSTRAINT [DFPersonSkill_ModificationDate]  DEFAULT (getdate()) FOR [ModifiedDate]
