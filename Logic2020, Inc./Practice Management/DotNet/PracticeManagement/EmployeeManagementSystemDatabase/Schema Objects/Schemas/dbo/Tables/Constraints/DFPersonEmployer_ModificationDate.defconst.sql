ALTER TABLE [dbo].[PersonEmployer]
ADD  CONSTRAINT [DFPersonEmployer_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
