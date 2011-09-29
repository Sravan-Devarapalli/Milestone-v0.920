CREATE TABLE [dbo].[Person](
	[PersonId] [bigint] IDENTITY(1,1) NOT NULL,
	[Alias] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[IsManager] [bit] NOT NULL,
	[TitleId] [int] NULL,
	[ManagerId] [bigint] NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)
)
