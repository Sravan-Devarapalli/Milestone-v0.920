-- =============================================
-- Author:		Srinivas.M
-- Create date: 06-05-2012
-- Updated By:	
-- Updated Date: 
-- Description:	Return whether user is Project Owner or not.
-- =============================================
CREATE PROCEDURE [dbo].[IsUserIsProjectOwner]
(
	@UserLogin NVARCHAR(100),
	@Id INT
)
AS
BEGIN
	
	DECLARE @PersonId INT
	
	SELECT @PersonId = PersonId
	FROM Person
	WHERE Alias = @UserLogin

	IF EXISTS (SELECT *
				FROM Project P
				WHERE ProjectManagerId = @PersonId)
	BEGIN
		SELECT 'True'
	END
	ELSE
	BEGIN
		SELECT 'False'
	END

END
