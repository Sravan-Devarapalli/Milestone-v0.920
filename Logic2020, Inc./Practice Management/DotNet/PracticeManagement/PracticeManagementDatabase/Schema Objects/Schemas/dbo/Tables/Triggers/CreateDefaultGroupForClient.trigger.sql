
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

	DECLARE @IsInternal BIT,
		@ProjectGroupCode NVARCHAR (5)
				
	SELECT @IsInternal = i.IsInternal FROM inserted AS i

	EXEC [GenerateNewProjectGroupCode] @IsInternal, @ProjectGroupCode OUTPUT

	INSERT INTO dbo.ProjectGroup (
		ClientId,
		[Name],
		Code
	)  ( 
		SELECT i.ClientId, 'Default Group' AS [Name], @ProjectGroupCode 
		FROM inserted AS i
	) 

END

