/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

EXEC sp_addmessage @msgnum = 70001, @severity = 16, @msgtext = N'There is another Person with the same First Name and Last Name.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70002, @severity = 16, @msgtext = N'There is another Person with the same Email.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70003, @severity = 16, @msgtext = N'The recruiting commission for the same period already exists.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70004, @severity = 16, @msgtext = N'There is another Client with the same Name.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70005, @severity = 16, @msgtext = N'The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70006, @severity = 16, @msgtext = N'Cannot inactivate the person who takes part in active milestones.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70007, @severity = 16, @msgtext = N'The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70008, @severity = 16, @msgtext = N'The period is incorrect. There records falls into the period specified in an existing record.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70009, @severity = 16, @msgtext = N'There is another Person with the same Employee Number.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70010, @severity = 16, @msgtext = N'The Expense category cannot be deleted because it is in use.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70011, @severity = 16, @msgtext = N'There is another Person with the same User Name.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70012, @severity = 16, @msgtext = N'The Expense category was deleted.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70013, @severity = 16, @msgtext = N'The Start Date is incorrect. There are several other default recruiter commissions for the specified period. Please edit them first.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70014, @severity = 16, @msgtext = N'The End Date is incorrect. There are several other default recruiter commissions for the specified period. Please edit them first.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70015, @severity = 16, @msgtext = N'The period is incorrect. There are several other default recruiter commissions for the specified period. Please edit them first.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70016, @severity = 16, @msgtext = N'The specified person is already assigned on this milestone.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70017, @severity = 16, @msgtext = N'This milestone cannot be deleted, because there are time entries related to it.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70018, @severity = 16, @msgtext = N'This milestone cannot be deleted, because there are project expenses related to it.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70019, @severity = 16, @msgtext = N'This person cannot be removed from milestone, because there (s)he has reported hours related to it.', @replace = 'replace'
EXEC sp_addmessage @msgnum = 70020, @severity = 16, @msgtext = N'There is another opportunity with same name.', @replace = 'replace'
GO


DECLARE @sql AS NVARCHAR(MAX);
DECLARE @newline AS NVARCHAR(2);
DECLARE @user_name AS NVARCHAR(100);

SET @newline = NCHAR(13) + NCHAR(10);
SET @user_name = N'PracticeManager';

SET @sql = N''
SELECT @sql = @sql
              + N'GRANT EXECUTE ON '
              + QUOTENAME(OBJECT_SCHEMA_NAME([object_id])) + '.'
              + QUOTENAME([name])
              + N' TO '
              + QUOTENAME(@user_name)
              + N';'
              + @newline + @newline
  FROM sys.procedures
  ORDER BY [name]

EXEC sp_executesql @sql;

SET @sql = N''
SELECT @sql = @sql
              + N'GRANT SELECT ON '
              + QUOTENAME(OBJECT_SCHEMA_NAME([object_id])) + '.'
              + QUOTENAME([name])
              + N' TO '
              + QUOTENAME(@user_name)
              + N';'
              + @newline + @newline
  FROM sys.views
  ORDER BY [name]

EXEC sp_executesql @sql;
  
SET @sql = N''
SELECT @sql = @sql
              + N'GRANT EXECUTE ON '
              + QUOTENAME(OBJECT_SCHEMA_NAME([object_id])) + '.'
              + QUOTENAME([name])
              + N' TO '
              + QUOTENAME(@user_name)
              + N';'
              + @newline + @newline
FROM sys.objects AS obj
WHERE obj.[type] IN (N'FN', N'IF', N'FS', N'FT')
ORDER BY [name]

EXEC sp_executesql @sql;

--PRINT @sql;

GO
