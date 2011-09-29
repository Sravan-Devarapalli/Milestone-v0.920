ALTER TABLE [dbo].[PersonTraining] ADD  CONSTRAINT [DFPersonTraining_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
