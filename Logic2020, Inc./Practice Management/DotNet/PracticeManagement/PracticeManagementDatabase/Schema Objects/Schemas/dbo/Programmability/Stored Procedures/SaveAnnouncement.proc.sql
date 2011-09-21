CREATE PROCEDURE [dbo].[SaveAnnouncement]
	@Text	NVARCHAR(MAX),
	@RichText	NVARCHAR(MAX)
AS
	DECLARE @date DATETIME

	SET @date = dbo.InsertingTime()

	IF NOT EXISTS (SELECT 1 FROM Announcements)
	BEGIN
		INSERT INTO Announcements(Text, RichText, Date)
		VALUES (@Text, @RichText, @date)
	END
	ELSE
	BEGIN
		UPDATE Announcements
		SET RichText = @RichText, [Text] = @Text
	END
