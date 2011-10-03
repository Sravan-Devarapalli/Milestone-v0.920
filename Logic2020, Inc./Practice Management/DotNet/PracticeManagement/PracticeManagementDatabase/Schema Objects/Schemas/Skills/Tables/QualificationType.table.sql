﻿CREATE TABLE [Skills].[QualificationType](
	[QualificationTypeId]	[int] NOT NULL IDENTITY(1,1), 
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_QualificationType] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[QualificationTypeId] ASC
)
)

