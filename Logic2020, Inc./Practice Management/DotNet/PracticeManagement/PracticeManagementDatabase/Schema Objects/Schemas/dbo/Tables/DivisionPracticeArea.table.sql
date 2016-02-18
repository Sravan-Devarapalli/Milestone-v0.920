CREATE TABLE [dbo].[DivisionPracticeArea]
(
	[Division_Practice_Id]	UNIQUEIDENTIFIER,
	[DivisionId]			INT,
	[PracticeId]			INT
	CONSTRAINT [PK_DivisionPracticeArea_Division_Practice_Id]	PRIMARY KEY (Division_Practice_Id)
)

