ALTER TABLE [dbo].[PersonQualification] ADD  CONSTRAINT [DFPersonQualification_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
