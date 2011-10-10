ALTER TABLE [Skills].[PersonDocument] 
ADD  CONSTRAINT [DFPersonDocument_ModificationDate]  DEFAULT (GETDATE()) FOR [ModifiedDate]
