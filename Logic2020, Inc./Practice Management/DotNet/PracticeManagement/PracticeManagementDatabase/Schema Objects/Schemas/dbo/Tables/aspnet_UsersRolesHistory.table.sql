CREATE TABLE [dbo].[aspnet_UsersRolesHistory](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[StartDate]	DATETIME NOT NULL,
	[EndDate]	DATETIME
	CONSTRAINT PK_UsersRolesHistory PRIMARY KEY CLUSTERED
	(
		[UserId] ASC,
		[RoleId] ASC,
		[StartDate]	ASC
	),
	CONSTRAINT FK_UsersRolesHistory_UserId FOREIGN KEY ([UserId])
	REFERENCES [dbo].[aspnet_Users] ([UserId]),
	CONSTRAINT FK_UsersRolesHistory_RoleId FOREIGN KEY (RoleId)
	REFERENCES dbo.aspnet_Roles(RoleId)
)

