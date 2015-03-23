CREATE VIEW [dbo].[v_PersonValidAttributionRange]
	AS 
	WIth PersonPayWithHistory
	AS
	(
	SELECT ph.PersonId,
	  CASE WHEN ph.HireDate < pay.StartDate THEN pay.StartDate ELSE ph.HireDate END AS Startdate,
	  CASE WHEN ph.PersonStatusId = 2 AND ph.TerminationDate < pay.EndDate THEN ph.TerminationDate ELSE pay.EndDate END AS Enddate
	FROM v_PersonHistory ph
	INNER JOIN v_paytimescalehistory pay ON ph.PersonId = pay.personid AND ph.HireDate <= pay.EndDate AND (ph.PersonStatusId <> 2 OR ph.TerminationDate IS NULL OR  pay.StartDate <= ph.TerminationDate)
	WHERE pay.Timescale IN (1,2) --W2Salary,W2Hourly
	)
	SELECT ph.PersonId,
	  CASE WHEN ph.StartDate < div.StartDate THEN div.StartDate ELSE ph.StartDate END AS Startdate,
	  CASE WHEN div.EndDate IS NOT NULL AND ph.EndDate > DATEADD(DD,-1,div.EndDate) THEN DATEADD(DD,-1,div.EndDate) ELSE ph.EndDate END AS Enddate
	FROM PersonPayWithHistory ph
	INNER JOIN v_DivisionHistory div ON ph.PersonId = div.personid AND (div.EndDate IS NULL OR ph.StartDate < div.EndDate) AND div.StartDate <= ph.EndDate
	WHERE ISNULL(div.DivisionId,0) IN (1,2,5,6,7) --Consulting,BusinessDevelopment

