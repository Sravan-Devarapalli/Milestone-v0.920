CREATE TABLE [dbo].[ProjectManagers]
(
	Id                      INT IDENTITY(1,1) NOT NULL,
	ProjectId	            INT NOT NULL, 
	ProjectManagerId		INT NOT NULL
	CONSTRAINT PK_ProjectManagers_Id PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_ProjectManagers_ProjectId FOREIGN KEY(ProjectId) REFERENCES  dbo.Project(ProjectId),
	CONSTRAINT FK_ProjectManagers_ProjectManagerId FOREIGN KEY(ProjectManagerId) REFERENCES  dbo.Person(PersonId)
);
