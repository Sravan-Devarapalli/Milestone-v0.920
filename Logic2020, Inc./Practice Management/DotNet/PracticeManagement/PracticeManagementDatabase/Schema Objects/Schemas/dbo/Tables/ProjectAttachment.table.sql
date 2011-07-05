CREATE TABLE dbo.ProjectAttachment (
	Id							INT IDENTITY(1,1) NOT NULL,
	ProjectId					INT				  NOT NULL,
	[FileName]					[nvarchar](256)       NULL,	
	[AttachmentData]			[varbinary](max)      NULL,
	UploadedDate				DATETIME			  NULL
   CONSTRAINT PK_ProjectSOW_Id PRIMARY KEY CLUSTERED(Id),
   CONSTRAINT FK_ProjectSOW_ProjectId FOREIGN KEY(ProjectId) REFERENCES  dbo.Project(ProjectId)
);
