CREATE TABLE [dbo].[Lockout]
(
	LockoutId			INT IDENTITY(1,1) NOT NULL,
	LockoutPageId		INT	NOT NULL,
	FunctionalityName	NVARCHAR(50) NOT NULL,
	Lockout				BIT	NOT NULL,
	LockoutDate			DATETIME NULL
)

