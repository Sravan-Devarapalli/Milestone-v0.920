CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1]
    ON [dbo].[aspnet_Roles]([ApplicationId] ASC, [LoweredRoleName] ASC) WITH ( DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


