ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [DF_Pay_DefaultHoursPerDay] DEFAULT ((8)) FOR [DefaultHoursPerDay];


