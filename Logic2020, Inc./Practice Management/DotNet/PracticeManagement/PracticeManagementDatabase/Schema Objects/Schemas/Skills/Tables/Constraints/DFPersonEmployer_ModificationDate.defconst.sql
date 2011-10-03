ALTER TABLE [Skills].[PersonEmployer]
ADD  CONSTRAINT [DFPersonEmployer_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
