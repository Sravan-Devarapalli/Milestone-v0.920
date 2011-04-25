CREATE TABLE [dbo].[UserTemporaryCredentials]
(
	[UserName]                               NVARCHAR (256)	  NOT NULL,
	[Password]                               NVARCHAR (128)   NOT NULL,
	[PasswordFormat]                         INT              DEFAULT ((0)) NOT NULL,
	[PasswordSalt]                           NVARCHAR (128)   NOT NULL,
	[CreatedDate]							 DATETIME		  NOT NULL,
	CONSTRAINT PK_UserTemporaryCredentials PRIMARY KEY  CLUSTERED ([UserName])
)

