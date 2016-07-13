CREATE PROCEDURE [dbo].[GetListOfChannels]
AS
BEGIN
	SELECT C.ChannelId,
		   C.ChannelName,
		   C.IsDefault
	FROM Channel C
	ORDER BY C.ChannelName
END
