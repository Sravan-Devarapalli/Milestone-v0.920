ALTER TABLE [dbo].[MinimumLoadFactorHistory]  
ADD  CONSTRAINT [FK_MinimumLoadFactorHistory_OverheadFixedRateId] FOREIGN KEY([OverheadFixedRateId])
REFERENCES [dbo].[OverheadFixedRate] ([OverheadFixedRateId])

