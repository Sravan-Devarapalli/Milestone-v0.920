CREATE TABLE [dbo].[ClientMarginColorInfo]
(
	Id              INT IDENTITY(1,1) NOT NULL,
	ClientId		INT NOT NULL,
	ColorId		    INT NOT NULL,
	StartRange		INT NOT NULL,
	EndRange		INT NOT NULL
	
	CONSTRAINT PK_ClientMarginColorInfo_Id PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_ClientMarginColorInfo_ClientId FOREIGN KEY(ClientId) REFERENCES  dbo.Client(ClientId),
	CONSTRAINT FK_ClientMarginColorInfo_ColorId FOREIGN KEY(ColorId) REFERENCES  dbo.Color([Id])
); 
