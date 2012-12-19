CREATE TABLE [dbo].[TitleType]
(
	TitleTypeId INT	NOT NULL,
	TitleType NVARCHAR(100) NULL,
	SortOrder		INT NOT NULL CONSTRAINT DF_TitleType_SortOrder DEFAULT(0),
	CONSTRAINT PK_TitleType_TitleTypeId     PRIMARY KEY CLUSTERED(TitleTypeId)
)

