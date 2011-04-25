ALTER TABLE [dbo].[OverheadFixedRate]
    ADD CONSTRAINT [FK_OverheadFixedRate_RateType] FOREIGN KEY ([RateType]) REFERENCES [dbo].[OverheadRateType] ([OverheadRateTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


