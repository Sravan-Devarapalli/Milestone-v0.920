ALTER TABLE [dbo].[RecruiterCommission]
    ADD CONSTRAINT [FK_RecruiterCommission_Recruiter] FOREIGN KEY ([RecruiterId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


