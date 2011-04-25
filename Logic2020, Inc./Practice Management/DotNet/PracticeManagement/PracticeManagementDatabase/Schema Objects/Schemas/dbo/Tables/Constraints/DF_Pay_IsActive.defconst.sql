ALTER TABLE [dbo].[Pay] ADD  CONSTRAINT [DF_Pay_IsActive]
  DEFAULT ((0)) FOR [IsActivePay]
