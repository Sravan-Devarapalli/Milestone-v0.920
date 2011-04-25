ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [DF_Pay_BonusAmount] DEFAULT ((0)) FOR [BonusAmount];


