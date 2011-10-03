ALTER TABLE [Skills].[PersonIndustry]
ADD  CONSTRAINT [DFPersonIndustry_ModificationDate]  DEFAULT (getdate()) FOR [ModificationDate]
