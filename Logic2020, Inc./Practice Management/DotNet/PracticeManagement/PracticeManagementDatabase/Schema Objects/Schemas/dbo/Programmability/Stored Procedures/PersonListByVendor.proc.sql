CREATE PROCEDURE [dbo].[PersonListByVendor]
(
	@VendorId INT
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Now DATETIME
	SET @Now = GETDATE()

			 SELECT A.* FROM( SELECT   p.PersonId,
					   ISNULL(p.PreferredFirstName,p.FirstName) AS FirstName,
					   p.LastName,
					   p.HireDate,
					   p.TerminationDate,
					   p.Alias,
					   p.DefaultPractice AS PracticeId,
					   p.PracticeName,
					   p.DivisionId,
			           p.DivisionName,
					   p.PersonStatusId,
		               p.PersonStatusName,
					 
		               p.TitleId,
		               p.Title,
					   pay.PersonId as PayPersonId,
					   pay.StartDate,
					   pay.EndDate,
					   
					   pay.Timescale,
					   pay.TimescaleName,
					   ROW_NUMBER() OVER(PARTITION BY p.PersonId ORDER BY p.PersonId) rno
			FROM v_Person p
			LEFT JOIN v_Pay pay ON p.PersonId=pay.PersonId
			WHERE pay.VendorId=@VendorId 
			AND ((@Now >= pay.StartDate AND @Now < pay.EndDateOrig) OR (NOT EXISTS(SELECT 1
	                    FROM dbo.v_Pay AS pa
	                   WHERE pa.PersonId = p.PersonId
	                     AND @Now >= pa.StartDate
	                     AND @Now < pa.EndDateOrig)
	   AND @Now < pay.StartDate))) A
			where A.rno=1

END
