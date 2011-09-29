ALTER TABLE [dbo].[PersonContactInfo]
ADD  CONSTRAINT [DFPersonContactInfo_ModificationDate]  
	DEFAULT (getdate()) FOR [ModificationDate]
