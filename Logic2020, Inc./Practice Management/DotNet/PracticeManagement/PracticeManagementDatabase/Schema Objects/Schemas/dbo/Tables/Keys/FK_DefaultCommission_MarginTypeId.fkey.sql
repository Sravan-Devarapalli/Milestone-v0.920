ALTER TABLE [dbo].[DefaultCommission]
    ADD CONSTRAINT [FK_DefaultCommission_MarginTypeId] FOREIGN KEY ([MarginTypeId]) REFERENCES [dbo].[MarginType] ([MarginTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


