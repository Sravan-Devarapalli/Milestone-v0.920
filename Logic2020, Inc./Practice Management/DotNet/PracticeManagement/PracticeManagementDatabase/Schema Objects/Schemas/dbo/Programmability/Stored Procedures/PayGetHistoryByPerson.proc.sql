--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-23-2008
-- Update by:	Anatoliy Lokshin
-- Update date:	6-24-2008
-- Description:	Retrives a payment histiry for the specified person.
-- =============================================
CREATE PROCEDURE [dbo].[PayGetHistoryByPerson]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

	SELECT p.PersonId,
	       p.StartDate,
	       p.EndDate,
	       p.Amount,
	       p.Timescale,
	       p.AmountHourly,
	       p.VacationDays,
	       p.BonusAmount,
	       p.BonusHoursToCollect,
	       p.IsYearBonus,
	       p.TimescaleName,
		   p.PracticeId,
		   pr.Name PracticeName,
		   Tt.TitleId,
		   Tt.Title,
		   p.SLTApproval,
		   p.SLTPTOApproval
	  FROM dbo.v_Pay AS p
	  LEFT JOIN dbo.Title Tt ON Tt.TitleId = P.TitleId
	  LEFT JOIN dbo.Practice pr on pr.PracticeId = p.PracticeId
	 WHERE p.PersonId = @PersonId
	ORDER BY p.StartDate

