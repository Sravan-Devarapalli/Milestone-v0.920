ALTER TABLE [Skills].[PersonTraining] ADD  CONSTRAINT [DFPersonTraining_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
