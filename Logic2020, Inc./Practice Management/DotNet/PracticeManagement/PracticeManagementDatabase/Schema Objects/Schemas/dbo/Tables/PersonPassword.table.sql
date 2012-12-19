CREATE TABLE [dbo].[PersonPassword]
(
	personId int NOT NULL, 
	password NVARCHAR(128) NOT NULL,
	CONSTRAINT PK_PersonPassword PRIMARY KEY  (personId)
)

