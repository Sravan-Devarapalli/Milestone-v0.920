ALTER TABLE [dbo].[MilestonePersonEntry]
    ADD CONSTRAINT [PK_MilestonePersonEntry] PRIMARY KEY CLUSTERED ([MilestonePersonId] ASC, [StartDate] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


