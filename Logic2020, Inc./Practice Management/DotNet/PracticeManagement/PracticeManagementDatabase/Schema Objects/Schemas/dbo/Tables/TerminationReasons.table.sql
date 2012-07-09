CREATE TABLE [dbo].[TerminationReasons]
(
	TerminationReasonId	INT IDENTITY(1,1),
	TerminationReason	NVARCHAR(2000)
	CONSTRAINT PK_TerminationReasons_TerminationReasonId PRIMARY KEY (TerminationReasonId)
)
