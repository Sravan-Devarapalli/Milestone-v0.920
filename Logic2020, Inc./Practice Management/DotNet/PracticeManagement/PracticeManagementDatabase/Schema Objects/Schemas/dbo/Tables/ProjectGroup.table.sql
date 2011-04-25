CREATE TABLE [dbo].[ProjectGroup] (
    [GroupId]  INT            IDENTITY (0, 1) NOT NULL,
    [ClientId] INT            NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
	Active     BIT            NOT NULL CONSTRAINT DF_ProjectGroup_Active DEFAULT  1,
    PRIMARY KEY CLUSTERED ([GroupId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF),
    FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client] ([ClientId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

 
