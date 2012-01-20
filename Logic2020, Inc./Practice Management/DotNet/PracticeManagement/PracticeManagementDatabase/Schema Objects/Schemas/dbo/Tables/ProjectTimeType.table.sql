CREATE TABLE [dbo].[ProjectTimeType]
(
	[ProjectId]			INT NOT NULL,
	[TimeTypeId]        INT NOT NULL,
	[IsAllowedToShow]   BIT NOT NULL,
	CONSTRAINT PK_ProjectTimeType_ProjectIdAndTimeTypeId PRIMARY KEY CLUSTERED ([ProjectId] ASC,[TimeTypeId] ASC)
);

