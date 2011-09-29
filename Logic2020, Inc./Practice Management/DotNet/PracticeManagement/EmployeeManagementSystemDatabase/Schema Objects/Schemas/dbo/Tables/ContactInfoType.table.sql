CREATE TABLE [dbo].[ContactInfoType](
	[ContactInfoTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_ContactInfoType] PRIMARY KEY CLUSTERED 
(
	[ContactInfoTypeId] ASC
)
)
