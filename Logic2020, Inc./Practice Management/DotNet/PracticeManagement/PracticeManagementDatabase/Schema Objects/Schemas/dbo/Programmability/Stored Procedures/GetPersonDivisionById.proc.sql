CREATE PROCEDURE [dbo].[GetPersonDivisionById]
(
@DivisionId INT
)
	
AS
BEGIN
	SET NOCOUNT ON	
	SELECT Pd.DivisionId,
		   Pd.DivisionName,
	       Pd.Inactive,
	       pd.ShowSetPracticeOwnerLink
	FROM   PersonDivision Pd 
	WHERE  Pd.DivisionId=@DivisionId
END
