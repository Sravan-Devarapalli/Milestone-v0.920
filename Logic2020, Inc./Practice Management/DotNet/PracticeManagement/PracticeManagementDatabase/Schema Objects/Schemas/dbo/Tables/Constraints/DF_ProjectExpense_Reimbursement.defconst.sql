ALTER TABLE [dbo].[ProjectExpense]
    ADD CONSTRAINT [DF_ProjectExpense_Reimbursement] DEFAULT ((0)) FOR [Reimbursement];


