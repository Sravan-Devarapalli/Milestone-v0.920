--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Gets a person's commision for the specific project
-- =============================================
CREATE PROCEDURE [dbo].[CommissionGetByProjectType]
(
	@ProjectId        INT,
	@CommissionType   INT
)
AS
	SET NOCOUNT ON

	SELECT c.ProjectId,
	       c.PersonId,
	       c.CommissionId,
	       c.FractionOfMargin,
	       c.CommissionType,
	       c.ExpectedDatePaid,
	       c.ActualDatePaid,
	       c.MarginTypeId
	  FROM dbo.Commission AS c
	 WHERE c.ProjectId = @ProjectId
	   AND c.CommissionType = @CommissionType

