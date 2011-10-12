CREATE FUNCTION [dbo].GetProjectManagerList
(
	@ProjectId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @rep NVARCHAR(MAX)
	SET @rep = ''

    DECLARE @IdSeperator NVARCHAR(255), @LastNameSeperator NVARCHAR(255),@FirstNameSeperator NVARCHAR(255) 
	SELECT @IdSeperator ='48429914-f383-4399-96c0-db719db82765',@LastNameSeperator ='8585ebd9-f14a-4729-9322-b0d834913e2e',@FirstNameSeperator='bc4ad2a9-2105-48b9-85e8-448408ba2a7a'
    
	SELECT @rep= @rep + CONVERT(NVARCHAR(255),pm.ProjectManagerId) + @IdSeperator + p.FirstName + @FirstNameSeperator + p.LastName + @LastNameSeperator 
    FROM dbo.Project AS pr 
    JOIN dbo.ProjectManagers AS pm ON pr.ProjectId = pm.ProjectId 
	JOIN dbo.Person AS p ON p.PersonId = pm.ProjectManagerId
    WHERE pr.ProjectId = @ProjectId

	RETURN @rep

END
