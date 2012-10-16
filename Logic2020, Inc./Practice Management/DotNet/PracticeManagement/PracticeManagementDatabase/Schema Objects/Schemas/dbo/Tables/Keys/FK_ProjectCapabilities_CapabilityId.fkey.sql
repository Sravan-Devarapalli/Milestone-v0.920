ALTER TABLE [dbo].[ProjectCapabilities]
ADD CONSTRAINT [FK_ProjectCapabilities_CapabilityId] FOREIGN KEY ([CapabilityId]) REFERENCES [dbo].[PracticeCapabilities]([CapabilityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
