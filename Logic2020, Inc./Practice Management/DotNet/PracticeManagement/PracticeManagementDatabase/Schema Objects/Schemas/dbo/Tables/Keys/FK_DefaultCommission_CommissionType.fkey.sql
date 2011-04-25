ALTER TABLE [dbo].[DefaultCommission]
    ADD CONSTRAINT [FK_DefaultCommission_CommissionType] FOREIGN KEY ([type]) REFERENCES [dbo].[CommissionType] ([CommissionTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


