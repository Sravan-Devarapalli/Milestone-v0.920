CREATE PROCEDURE [dbo].[ReBuildIndexes]
AS
BEGIN
 
		EXEC sp_updatestats
 
		DECLARE @TableName varchar(255) 
		DECLARE TableCursor CURSOR FOR 
		SELECT TABLE_NAME FROM information_schema.tables
		WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA != 'DataSync' AND TABLE_SCHEMA = 'dbo' 
		AND TABLE_NAME IN ('TimeEntryHistory',
							'OpportunityPersons',
							'Calendar',
							'PersonCalendar',
							'PersonCalendarAuto',
							'MilestonePersonEntry',
							'PersonHistory',
							'UserActivityLog',
							'WorkinHoursByYear',
							'Project',
							'PracticeCapabilities',
							'TimeType',
							'MilestonePerson',
							'ChargeCode',
							'ChargeCodeTurnOffHistory',
							'PersonTimeEntryRecursiveSelection',
							'ProjectTimeType',
							'TimeEntry',
							'TimeEntryHours')


		OPEN TableCursor 
		FETCH NEXT FROM TableCursor INTO @TableName 
		WHILE @@FETCH_STATUS = 0 
		BEGIN 
		exec('ALTER INDEX ALL ON ' + @TableName + ' REBUILD')
		FETCH NEXT FROM TableCursor INTO @TableName 
		print @TableName
		END 

		CLOSE TableCursor 
		DEALLOCATE TableCursor

END

