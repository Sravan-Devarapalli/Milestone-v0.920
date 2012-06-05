CREATE TABLE [dbo].[ProjectAttachmentCategory]
(
	Id							INT IDENTITY(1,1) NOT NULL,
	Name						nvarchar(256)       NULL,
	CONSTRAINT PK_ProjectAttachmentCategory_Id PRIMARY KEY CLUSTERED(Id)	
)

