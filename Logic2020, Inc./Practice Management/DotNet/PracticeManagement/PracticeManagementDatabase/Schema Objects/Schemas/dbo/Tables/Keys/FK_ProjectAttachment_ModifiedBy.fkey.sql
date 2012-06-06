ALTER TABLE [dbo].[ProjectAttachment]
    ADD CONSTRAINT [FK_ProjectAttachment_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


