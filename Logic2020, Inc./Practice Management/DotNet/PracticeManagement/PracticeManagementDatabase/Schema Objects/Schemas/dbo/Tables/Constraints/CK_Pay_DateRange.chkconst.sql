ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [CK_Pay_DateRange] CHECK ([StartDate]<[EndDate]);


