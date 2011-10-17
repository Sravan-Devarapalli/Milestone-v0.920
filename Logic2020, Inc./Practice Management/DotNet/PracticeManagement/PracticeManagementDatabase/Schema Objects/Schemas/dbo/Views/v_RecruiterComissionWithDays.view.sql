CREATE VIEW [dbo].[v_RecruiterComissionWithDays]
AS
WITH Ordered AS (
	SELECT
			ROW_NUMBER() OVER (PARTITION BY p.PersonId ORDER BY p.PersonId ASC) AS rownum, 
			rec.PersonId AS RecruiterId, rec.FirstName + ' ' + rec.LastName AS RecruiterName, rcomm1.RecruitId,
			rcomm1.Amount AS cc1, rcomm1.HoursToCollect / 24 AS cd1, 
			rcomm2.Amount AS cc2, rcomm2.HoursToCollect / 24 AS cd2
			FROM dbo.Person AS p 
			INNER JOIN dbo.RecruiterCommission rcomm1 ON p.PersonId = rcomm1.RecruitId
			LEFT JOIN dbo.RecruiterCommission rcomm2 ON p.PersonId = rcomm2.RecruitId  AND rcomm1.HoursToCollect != rcomm2.HoursToCollect
			INNER JOIN dbo.Person AS rec ON rcomm1.RecruiterId = rec.PersonId
			
)
SELECT * FROM Ordered WHERE rownum % 2 = 1
