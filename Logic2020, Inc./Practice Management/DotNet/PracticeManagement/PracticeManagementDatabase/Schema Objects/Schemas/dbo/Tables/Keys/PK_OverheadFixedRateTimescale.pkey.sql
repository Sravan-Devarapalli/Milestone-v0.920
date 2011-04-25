ALTER TABLE [dbo].[OverheadFixedRateTimescale]
    ADD CONSTRAINT [PK_OverheadFixedRateTimescale] PRIMARY KEY CLUSTERED ([OverheadFixedRateId] ASC, [TimescaleId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


