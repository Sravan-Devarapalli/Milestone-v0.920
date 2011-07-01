CREATE TABLE [dbo].[DefaultMarginColorInfo]
(
	Id              INT IDENTITY(1,1) NOT NULL,
	GoalTypeId		INT NOT NULL,
	ColorId		    INT NOT NULL,
	StartRange		INT NOT NULL,
	EndRange		INT NOT NULL
	CONSTRAINT PK_DefaultMarginColorInfo_Id PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_DefaultMarginColorInfo_GoalTypeId FOREIGN KEY(GoalTypeId) REFERENCES  dbo.DefaultGoalType(Id),
	CONSTRAINT FK_DefaultMarginColorInfo_ColorId FOREIGN KEY(ColorId) REFERENCES  dbo.Color([Id])
);

