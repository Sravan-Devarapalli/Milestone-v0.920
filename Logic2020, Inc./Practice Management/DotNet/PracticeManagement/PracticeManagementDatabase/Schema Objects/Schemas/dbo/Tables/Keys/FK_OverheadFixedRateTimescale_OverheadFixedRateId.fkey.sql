ALTER TABLE [dbo].[OverheadFixedRateTimescale]
    ADD CONSTRAINT [FK_OverheadFixedRateTimescale_OverheadFixedRateId] FOREIGN KEY ([OverheadFixedRateId]) REFERENCES [dbo].[OverheadFixedRate] ([OverheadFixedRateId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


