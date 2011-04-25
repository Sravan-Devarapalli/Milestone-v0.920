CREATE UNIQUE NONCLUSTERED INDEX [IX_DefaultRecruiterCommissionHeader_PersonStartDate]
    ON [dbo].[DefaultRecruiterCommissionHeader]([PersonId] ASC, [StartDate] ASC)
    INCLUDE([EndDate]) WITH ( DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


