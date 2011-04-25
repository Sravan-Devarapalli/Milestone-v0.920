ALTER TABLE [dbo].[DefaultRecruiterCommissionItem]
    ADD CONSTRAINT [FK_DefaultRecruiterCommissionItem_Id] FOREIGN KEY ([DefaultRecruiterCommissionHeaderId]) REFERENCES [dbo].[DefaultRecruiterCommissionHeader] ([DefaultRecruiterCommissionHeaderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


