-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-16-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects persons commissions on the projects. Uses for filters.
-- =============================================
CREATE VIEW [dbo].[v_PersonProjectCommission]
AS
	SELECT c.ProjectId, c.PersonId, c.CommissionId, p.Alias, c.CommissionType
	  FROM dbo.Commission AS c
	       INNER JOIN dbo.Person AS p ON p.PersonId = c.PersonId

