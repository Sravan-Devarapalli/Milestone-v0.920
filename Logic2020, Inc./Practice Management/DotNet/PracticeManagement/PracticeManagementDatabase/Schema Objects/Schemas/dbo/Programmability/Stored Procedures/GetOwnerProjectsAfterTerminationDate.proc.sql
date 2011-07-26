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
	WHERE proj.ProjectManagerId =@PersonId AND proj.ProjectStatusId =3 
	
END
