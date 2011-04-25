CREATE NONCLUSTERED INDEX [IX_OpportunityTransition_OpportunityId]
    ON [dbo].[OpportunityTransition]([OpportunityId] ASC) WITH ( DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


