CREATE TABLE [dbo].[ProjectDivisionPracticeMapping]
(
    [ProjectDivision_Practice_Id]	UNIQUEIDENTIFIER,
	[ProjectDivisionId]			INT,
	[PracticeId]			INT
	CONSTRAINT [PK_ProjectDivisionPracticeMapping_ProjectDivision_Practice_Id]	PRIMARY KEY (ProjectDivision_Practice_Id)
)

