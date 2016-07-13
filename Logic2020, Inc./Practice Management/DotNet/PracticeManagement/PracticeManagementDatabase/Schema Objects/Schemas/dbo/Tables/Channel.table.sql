CREATE TABLE [dbo].[Channel]
(
	[ChannelId]    INT             IDENTITY (1, 1) NOT NULL,
	[ChannelName]  NVARCHAR (50)   NOT NULL,
	[IsDefault]	   BIT			   NOT NULL CONSTRAINT DF_Channel_IsDefault DEFAULT 0,
	[IsSubChannelNamePicker]	   BIT			   NOT NULL CONSTRAINT DF_Channel_IsSubChannelNamePicker DEFAULT 0,
	[IsSubChannelVendor]			BIT				NOT NULL CONSTRAINT DF_Channel_IsSubChannelVendor DEFAULT 0,
	CONSTRAINT [PK_Channel_ChannelId]	PRIMARY KEY ([ChannelId])
)

