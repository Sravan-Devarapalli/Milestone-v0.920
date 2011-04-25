
-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-14
-- Description:	Creates default group on each
-- =============================================
CREATE TRIGGER DeleteDefaultGroupForClient 
   ON  dbo.Client 
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.ProjectGroup 
	WHERE ClientId IN ( 
			SELECT d.ClientId FROM deleted AS d
		)

END

