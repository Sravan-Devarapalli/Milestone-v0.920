CREATE TABLE [Skills].[ContactInfoType](
	[ContactInfoTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_ContactInfoType_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_ContactInfoType_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_ContactInfoType] PRIMARY KEY CLUSTERED 
(
	[ContactInfoTypeId] ASC
)
)
