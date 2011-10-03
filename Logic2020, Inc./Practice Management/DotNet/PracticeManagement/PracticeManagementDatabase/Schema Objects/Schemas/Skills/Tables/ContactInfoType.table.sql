﻿CREATE TABLE [Skills].[ContactInfoType](
	[ContactInfoTypeId] [int] NOT NULL IDENTITY(1,1),
	[TenantId]		[int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_ContactInfoType] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[ContactInfoTypeId] ASC
)
)
