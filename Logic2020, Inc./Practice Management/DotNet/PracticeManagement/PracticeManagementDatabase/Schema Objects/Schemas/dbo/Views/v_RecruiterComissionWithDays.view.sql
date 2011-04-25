CREATE VIEW [dbo].[v_RecruiterComissionWithDays]
AS
WITH Ordered AS (
	SELECT
			ROW_NUMBER() OVER (ORDER BY p.PersonId ASC) AS rownum, 
			rec.PersonId AS RecruiterId, rec.FirstName + ' ' + rec.LastName AS RecruiterName, rcomm1.RecruitId,
			rcomm1.Amount AS cc1, rcomm1.HoursToCollect / 24 AS cd1, 
			rcomm2.Amount AS cc2, rcomm2.HoursToCollect / 24 AS cd2
			FROM dbo.Person AS p 
			INNER JOIN dbo.RecruiterCommission rcomm1 ON p.PersonId = rcomm1.RecruitId
			INNER JOIN dbo.RecruiterCommission rcomm2 ON p.PersonId = rcomm2.RecruitId
			INNER JOIN dbo.Person AS rec ON rcomm2.RecruiterId = rec.PersonId
			WHERE (rcomm1.HoursToCollect != rcomm2.HoursToCollect OR 
					rcomm1.Amount != rcomm2.Amount)
)
SELECT * FROM Ordered WHERE rownum % 2 = 1
