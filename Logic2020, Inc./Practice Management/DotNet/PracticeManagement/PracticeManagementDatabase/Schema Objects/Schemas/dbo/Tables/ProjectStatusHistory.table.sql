CREATE TABLE [dbo].[ProjectStatusHistory]
(
	ProjectId		INT NOT NULL,
	ProjectStatusId	INT NOT NULL,
	StartDate		DATETIME NOT NULL,
	EndDate			DATETIME NULL,
	CONSTRAINT PK_ProjectStatusHistory PRIMARY KEY CLUSTERED
	(
		ProjectId ASC,	
		ProjectStatusId ASC,	
		StartDate ASC
	)
)

