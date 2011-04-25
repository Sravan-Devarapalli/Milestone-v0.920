ALTER TABLE [dbo].[DefaultCommission]
    ADD CONSTRAINT [PK_DefaultCommission] PRIMARY KEY CLUSTERED ([PersonId] ASC, [StartDate] ASC, [EndDate] ASC, [type] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


