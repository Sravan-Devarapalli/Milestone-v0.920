
-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-14
-- Description:	Creates default group on each
-- =============================================
CREATE TRIGGER CreateDefaultGroupForClient 
   ON  dbo.Client 
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.ProjectGroup (
		ClientId,
		[Name]
	)  ( 
		SELECT i.ClientId, 'Default Group' AS [Name] FROM inserted AS i
	) 

END

