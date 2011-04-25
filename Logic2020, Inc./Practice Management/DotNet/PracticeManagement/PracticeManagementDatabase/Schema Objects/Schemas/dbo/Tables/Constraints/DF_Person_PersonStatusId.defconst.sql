ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [DF_Person_PersonStatusId] DEFAULT ((1)) FOR [PersonStatusId];


