ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_Client] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client] ([ClientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


