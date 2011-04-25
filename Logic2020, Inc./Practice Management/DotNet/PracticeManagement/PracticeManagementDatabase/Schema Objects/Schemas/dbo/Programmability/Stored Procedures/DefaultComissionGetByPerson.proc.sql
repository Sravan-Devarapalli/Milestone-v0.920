--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Description:	Gets a person's default actual commisions
-- =============================================
CREATE PROCEDURE [dbo].[DefaultComissionGetByPerson]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON
	
	DECLARE @Today DATETIME
	SET @Today = dbo.[Today]()
	
	SELECT c.[PersonId],
	       c.[FractionOfMargin],
	       c.[StartDate],
	       c.[EndDate],
	       c.[type],
	       c.[MarginTypeId]
	  FROM dbo.DefaultCommission AS c
	 WHERE c.[PersonId] = @PersonId
	   AND c.[StartDate] <= @Today
	   AND c.[EndDate] > @Today

