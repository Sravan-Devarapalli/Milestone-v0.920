CREATE UNIQUE CLUSTERED INDEX [UN_DefaultMilestoneSetting]
ON [dbo].[DefaultMilestoneSetting](
									[ClientId] ASC,
									[ProjectId] ASC, 
									[MilestoneId] ASC,
									[ModifiedDate] ASC) WITH 
	( DROP_EXISTING = OFF,
	 IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];
