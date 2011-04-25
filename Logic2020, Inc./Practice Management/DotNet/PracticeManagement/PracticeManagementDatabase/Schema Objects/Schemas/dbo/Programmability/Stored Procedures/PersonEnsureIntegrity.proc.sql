-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-14-2008
-- Updated by:	
-- Update date: 
-- Description:	Verifies whether the person's data are consistent.
-- =============================================
CREATE PROCEDURE dbo.PersonEnsureIntegrity
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

	DECLARE @LastName NVARCHAR(40)
	DECLARE @FirstName NVARCHAR(40)
	DECLARE @HireDate DATETIME
	DECLARE @Message NVARCHAR(2000)
	DECLARE @RecList NVARCHAR(1000)

	-- Verify the recruiter commission
	SELECT TOP 1 @LastName = r.LastName, @FirstName = r.FirstName, @HireDate = p.HireDate
	  FROM dbo.RecruiterCommission AS c
	       INNER JOIN dbo.Person AS p ON c.RecruitId = p.PersonId
	       INNER JOIN dbo.Person AS r ON r.PersonId = c.RecruiterId
	 WHERE c.RecruitId = @PersonId
	   AND NOT EXISTS (SELECT 1
	                     FROM dbo.DefaultRecruiterCommissionHeader AS d
	                    WHERE d.PersonId = c.RecruiterId
	                      AND p.HireDate BETWEEN StartDate AND EndDate)

	IF @LastName IS NOT NULL
	BEGIN
		SET @Message = 'The desired recruiter ' + @LastName + ', ' + @FirstName + ' has no defualt recruiting commission for ' + CONVERT(NVARCHAR(20), @HireDate, 101)
		RAISERROR(@Message, 16, 1)

		-- Clean up selected data
		SELECT @LastName = NULL, @FirstName = NULL, @HireDate = NULL
	END

	
	set @RecList = ''

	-- Verify the default recruter commission
	--SELECT TOP 1 @LastName = p.LastName, @FirstName = p.FirstName, @HireDate = p.HireDate
	;WITH recruits as 
	(
		SELECT distinct p.*
		  FROM dbo.RecruiterCommission AS c
			   INNER JOIN dbo.Person AS p ON c.RecruitId = p.PersonId
		 WHERE c.RecruiterId = @PersonId
		   AND NOT EXISTS (SELECT 1
							 FROM dbo.DefaultRecruiterCommissionHeader AS d
							WHERE d.PersonId = c.RecruiterId
							  AND p.HireDate BETWEEN StartDate AND EndDate)
	)
	SELECT @RecList = @RecList + r.LastName + ' ' + r.FirstName + ', '
	  FROM recruits as r

	IF @RecList <> ''
	BEGIN
		--SET @Message = 'The person has no defualt recruiting commission for ' + CONVERT(NVARCHAR(20), @HireDate, 101) + ' but has a recruit ' + @LastName + ', ' + @FirstName
		SET @Message = 'The person has no default recruiting commission for the period but has a recruit(s): ' + @RecList
		RAISERROR(@Message, 16, 1)
	END

