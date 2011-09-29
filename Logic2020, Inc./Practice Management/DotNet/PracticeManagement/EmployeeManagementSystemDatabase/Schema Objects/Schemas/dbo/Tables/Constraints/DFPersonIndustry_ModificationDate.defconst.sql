ALTER TABLE [dbo].[PersonIndustry]
ADD  CONSTRAINT [DFPersonIndustry_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
