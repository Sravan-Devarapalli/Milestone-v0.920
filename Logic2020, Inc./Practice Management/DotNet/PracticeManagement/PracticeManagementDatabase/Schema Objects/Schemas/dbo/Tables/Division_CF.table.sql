CREATE TABLE [dbo].[Division_CF]
(
	DivisionId			INT			IDENTITY (1, 1) NOT NULL,
	DivisionCode		NVARCHAR(20)	NOT NULL,
	DivisionName		NVARCHAR(50)	NOT NULL,
	ParentId			INT				NULL
)

