CREATE PROCEDURE [dbo].[GetLatestAnnouncement]	
AS
	SELECT TOP 1 A.RichText
	FROM dbo.Announcements AS A
	ORDER BY A.[Date] DESC
