ALTER TABLE [dbo].[UserActivityLogRecordPerChange]
	ADD CONSTRAINT [PK_UserActivityLogRecordPerChange_ActivityId] PRIMARY KEY CLUSTERED ([ActivityID] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
