CREATE PROCEDURE [dbo].[GetProjectDivisions]
AS
BEGIN
	SELECT pd.DivisionId,
		   pd.DivisionName,
		   pd.IsExternal
		   FROM dbo.ProjectDivision pd
		   ORDER BY pd.DivisionName
END 
