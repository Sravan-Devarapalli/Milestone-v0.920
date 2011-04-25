ALTER TABLE [dbo].[Practice]
    ADD CONSTRAINT [DF_Practice_PracticeManagerId] DEFAULT ((1)) FOR [PracticeManagerId];


