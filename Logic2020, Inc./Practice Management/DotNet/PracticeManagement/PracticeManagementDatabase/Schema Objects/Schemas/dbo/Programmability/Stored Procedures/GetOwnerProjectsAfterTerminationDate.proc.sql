﻿CREATE PROCEDURE [dbo].[GetOwnerProjectsAfterTerminationDate]
(
	@PersonId	INT,
	@TerminationDate	DATETIME 
	
)
AS
BEGIN 
	--Returns the active project if the given personid is project Owner.

	SELECT proj.ProjectId,
		   proj.Name ProjectName
	FROM dbo.Project AS proj 
	WHERE proj.ProjectOwnerId = @PersonId AND proj.ProjectStatusId =3 --Active Status
	
END
