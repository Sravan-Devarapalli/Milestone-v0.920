ALTER TABLE [dbo].[ProjectAttachment]
    ADD CONSTRAINT [FK_ProjectAttachment_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[ProjectAttachmentCategory] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;


