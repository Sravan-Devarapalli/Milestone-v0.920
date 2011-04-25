ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [FK_Pay_Timescale] FOREIGN KEY ([Timescale]) REFERENCES [dbo].[Timescale] ([TimescaleId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


