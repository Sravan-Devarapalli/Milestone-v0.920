ALTER TABLE [dbo].[PersonDocument] 
ADD  CONSTRAINT [DFPersonDocument_ModificationDate]  DEFAULT (GETDATE()) FOR [ModificationDate]
