CREATE PROCEDURE [dbo].[SkillsReport]
AS
BEGIN
		SELECT	P.EmployeeNumber AS [Consultant ID],
				P.LastName+', '+P.FirstName AS [Consultant Name],
				PRA.Name AS [Practice Area],
				COUNT(*) AS [Skills Count],
				dbo.GetSkillsByPerson(PS.PersonId) AS Skills,
				dbo.GetIndustriesByPerson(PS.PersonId) AS Industries,
				PP.ProfileName,
				PP.ProfileUrl AS [Profile Link],
				MAX(PS.ModifiedDate) AS [Last Updated Date]
		FROM Skills.PersonSkill PS
		INNER JOIN dbo.Person P ON P.PersonId = PS.PersonId
		LEFT JOIN dbo.Practice PRA ON PRA.PracticeId = P.DefaultPractice
		LEFT JOIN Skills.PersonProfile PP ON PP.PersonId = P.PersonId AND PP.IsDefault = 1
		WHERE PRA.Name <> 'Administration' AND P.IsOffshore = 0
			  AND P.PersonStatusId IN (1,5)
		GROUP BY PS.PersonId,
				 P.FirstName,
				 P.LastName,
				 P.EmployeeNumber,
				 PRA.Name,
				 PP.ProfileName,
				 PP.ProfileUrl
		ORDER BY P.EmployeeNumber
END
