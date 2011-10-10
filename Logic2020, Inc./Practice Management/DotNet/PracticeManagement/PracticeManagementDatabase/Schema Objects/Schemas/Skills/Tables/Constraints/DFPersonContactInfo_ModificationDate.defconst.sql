ALTER TABLE [Skills].[PersonContactInfo]
ADD  CONSTRAINT [DFPersonContactInfo_ModificationDate]  
	DEFAULT (getdate()) FOR [ModifiedDate]
