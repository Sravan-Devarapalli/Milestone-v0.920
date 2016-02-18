CREATE PROCEDURE [dbo].[CheckIfPersonIsOwnerForDivisionAndOrPractice]
	@PersonId INT 
AS
BEGIN
	SELECT PD.DivisionName AS 'TargetName',
			CAST(1 AS BIT) 'IsDivisionOwner'
	FROM dbo.PersonDivision PD
	WHERE PD.DivisionOwnerId=@PersonId
	UNION ALL
	SELECT P.Name AS 'TargetName',
			CAST(0 AS BIT) AS 'IsDivisionOwner'
	FROM dbo.Practice P
	WHERE p.PracticeManagerId=@PersonId
END	
