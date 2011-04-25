ALTER TABLE [dbo].[DefaultRecruiterCommissionItem]
    ADD CONSTRAINT [PK_DefaultRecruiterCommissionItem] PRIMARY KEY CLUSTERED ([DefaultRecruiterCommissionHeaderId] ASC, [HoursToCollect] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


