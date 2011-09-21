CREATE TABLE [dbo].[QuickLinks]
(
	Id              INT IDENTITY(1,1) NOT NULL,
	LinkName	    NVARCHAR (255) NOT NULL, 
	VirtualPath		NVARCHAR (255) NOT NULL,
	DashBoardTypeId INT NOT NULL
	CONSTRAINT PK_QuickLinks_Id PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_QuickLinks_DashBoardTypeId FOREIGN KEY(DashBoardTypeId) REFERENCES  dbo.DashBoardType(Id)
); 
