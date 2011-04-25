ALTER TABLE [dbo].[TimeEntries]
    ADD CONSTRAINT [DF_TimeEntries_IsCorrect] DEFAULT ((1)) FOR [IsCorrect];


