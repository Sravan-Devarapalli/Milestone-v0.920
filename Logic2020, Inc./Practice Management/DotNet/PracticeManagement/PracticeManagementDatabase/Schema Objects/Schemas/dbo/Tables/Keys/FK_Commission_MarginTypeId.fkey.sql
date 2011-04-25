ALTER TABLE [dbo].[Commission]
    ADD CONSTRAINT [FK_Commission_MarginTypeId] FOREIGN KEY ([MarginTypeId]) REFERENCES [dbo].[MarginType] ([MarginTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


