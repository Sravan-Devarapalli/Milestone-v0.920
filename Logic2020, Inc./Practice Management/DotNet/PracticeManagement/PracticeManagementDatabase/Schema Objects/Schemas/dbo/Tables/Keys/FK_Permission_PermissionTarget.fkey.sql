ALTER TABLE [dbo].[Permission]
    ADD CONSTRAINT [FK_Permission_PermissionTarget] FOREIGN KEY ([TargetType]) REFERENCES [dbo].[PermissionTarget] ([PermissionTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


