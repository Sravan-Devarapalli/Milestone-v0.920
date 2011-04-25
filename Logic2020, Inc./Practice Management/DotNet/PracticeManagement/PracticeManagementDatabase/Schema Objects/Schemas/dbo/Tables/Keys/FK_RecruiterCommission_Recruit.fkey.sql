ALTER TABLE [dbo].[RecruiterCommission]
    ADD CONSTRAINT [FK_RecruiterCommission_Recruit] FOREIGN KEY ([RecruitId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


