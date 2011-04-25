ALTER TABLE [dbo].[RecruiterCommission]
    ADD CONSTRAINT [PK_RecruiterCommission_1] PRIMARY KEY CLUSTERED ([RecruitId] ASC,[RecruiterId] ASC, [HoursToCollect] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


