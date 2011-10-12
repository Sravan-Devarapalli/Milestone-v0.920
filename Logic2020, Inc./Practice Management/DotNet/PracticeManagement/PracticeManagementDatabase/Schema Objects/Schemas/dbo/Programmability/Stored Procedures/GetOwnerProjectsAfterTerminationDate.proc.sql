CREATE PROCEDURE [dbo].[GetOwnerProjectsAfterTerminationDate]
(
	@PersonId	INT,
	@TerminationDate	DATETIME 
	
)
AS
BEGIN 

	SELECT proj.ProjectId,
		   proj.Name ProjectName
	FROM dbo.Project AS proj 
	INNER JOIN dbo.ProjectManagers AS pm ON pm.ProjectId = proj.ProjectId
	WHERE pm.ProjectManagerId =@PersonId AND proj.ProjectStatusId =3 
	
END
