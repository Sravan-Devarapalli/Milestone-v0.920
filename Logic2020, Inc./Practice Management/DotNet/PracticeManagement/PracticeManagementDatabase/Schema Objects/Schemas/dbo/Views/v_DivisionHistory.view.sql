 CREATE VIEW [dbo].[v_DivisionHistory]
 AS 
 WITH PersonHistoryCTE
 AS 
 (
	 SELECT  ROW_NUMBER () OVER (partition by PH.PersonId,CONVERT(DATE,PH.CreatedDate) ORDER BY PH.CreatedDate desc) as DayRowNumber,
			 PH.PersonId,  
			 CONVERT(DATE,PH.CreatedDate) CreatedDate,
			 PH.DivisionId,
			 PH.HireDate
	 FROM dbo.PersonHistory PH
	 WHERE PH.IsStrawman = 0  
 )
 ,PersonHistoryDayWise
 AS
 (
	  SELECT *,ROW_NUMBER () OVER (partition by PH1.PersonId ORDER BY pH1.CreatedDate) as DivisionRowNumber
	  FROM PersonHistoryCTE PH1
	  WHERE PH1.DayRowNumber = 1
 )
 ,DivisionHistory
 AS
 (
	SELECT * ,ROW_NUMBER () OVER (partition by dh.PersonId ORDER BY dh.CreatedDate) as DivisionRank
	FROM 
	 (
		 SELECT ph1.PersonId,ph1.DivisionId,PH1.CreatedDate,PH1.HireDate
		  FROM PersonHistoryDayWise PH1
		  LEFT JOIN PersonHistoryDayWise PH2 ON ph1.PersonId = ph2.PersonId AND ph1.DivisionRowNumber = ph2.DivisionRowNumber+1 AND ISNULL(ph1.DivisionId,0) = ISNULL(ph2.DivisionId,0)
		  WHERE ph2.DivisionRowNumber IS NULL
	 ) AS dh
 )
 
 SELECT  dh1.PersonId,
		 CASE WHEN dh1.DivisionRank = 1 AND dh1.HireDate < dh1.CreatedDate THEN dh1.HireDate ELSE dh1.CreatedDate END AS StartDate,
		 dh2.CreatedDate AS EndDate,
		 dh1.DivisionId
 FROM DivisionHistory dh1
 LEFT JOIN DivisionHistory dh2 ON dh1.PersonId = dh2.PersonId AND dh1.DivisionRank + 1 = dh2.DivisionRank
