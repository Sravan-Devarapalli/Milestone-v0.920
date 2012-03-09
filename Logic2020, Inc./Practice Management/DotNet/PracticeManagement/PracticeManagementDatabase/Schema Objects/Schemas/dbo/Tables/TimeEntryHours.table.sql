CREATE TABLE [dbo].[TimeEntryHours]
(
    [Id]                INT			IDENTITY (1, 1) NOT NULL,
    [TimeEntryId]       INT 		NOT NULL,
    [ActualHours]       REAL		NOT NULL,
    [CreateDate]        DATETIME	NOT NULL,
    [ModifiedDate]      DATETIME	NOT NULL,
    [ModifiedBy]        INT			NOT NULL, 
    [IsChargeable]      BIT			NOT NULL,
    [ReviewStatusId]    INT			NOT NULL CONSTRAINT DF_TimeEntryHours_ReviewStatusId DEFAULT  1--default constraint to pending status.
	, CONSTRAINT PK_TimeEntryHours_TimeEntryIdAndId PRIMARY KEY CLUSTERED(TimeEntryId ASC ,Id ASC )
)

