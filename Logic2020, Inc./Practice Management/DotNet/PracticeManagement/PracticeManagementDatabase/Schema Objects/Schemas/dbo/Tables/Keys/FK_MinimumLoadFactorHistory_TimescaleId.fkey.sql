ALTER TABLE [dbo].[MinimumLoadFactorHistory]  
ADD  CONSTRAINT [FK_MinimumLoadFactorHistory_TimescaleId] FOREIGN KEY([TimescaleId])
REFERENCES [dbo].[Timescale] ([TimescaleId])
