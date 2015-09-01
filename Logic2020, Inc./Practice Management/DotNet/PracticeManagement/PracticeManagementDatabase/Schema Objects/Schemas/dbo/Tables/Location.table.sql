CREATE TABLE [dbo].[Location]
(
	LocationId		INT				NOT NULL,
	LocationCode	NVARCHAR(10)	NOT NULL,
	LocationName	NVARCHAR(30)	NULL,
	ParentId		INT				NULL,
	Country			NVARCHAR(20)	NULL,
	TimeZone		NVARCHAR(10)	NULL
)

