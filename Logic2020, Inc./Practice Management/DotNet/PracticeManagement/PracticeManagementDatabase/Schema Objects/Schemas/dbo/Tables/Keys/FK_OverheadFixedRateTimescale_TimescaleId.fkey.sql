ALTER TABLE [dbo].[OverheadFixedRateTimescale]
    ADD CONSTRAINT [FK_OverheadFixedRateTimescale_TimescaleId] FOREIGN KEY ([TimescaleId]) REFERENCES [dbo].[Timescale] ([TimescaleId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


