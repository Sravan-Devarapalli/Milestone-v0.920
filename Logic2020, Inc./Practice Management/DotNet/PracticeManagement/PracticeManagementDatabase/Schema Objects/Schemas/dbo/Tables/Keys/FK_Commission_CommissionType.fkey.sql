ALTER TABLE [dbo].[Commission]
    ADD CONSTRAINT [FK_Commission_CommissionType] FOREIGN KEY ([CommissionType]) REFERENCES [dbo].[CommissionType] ([CommissionTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


