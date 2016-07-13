CREATE PROCEDURE [dbo].[GetChannelByID]
(
	@ChannelId INT
)
AS
BEGIN
	SELECT C.ChannelId,
		   C.ChannelName,
		   C.IsSubChannelNamePicker
	FROM dbo.Channel C
	WHERE C.ChannelId=@ChannelId
END
