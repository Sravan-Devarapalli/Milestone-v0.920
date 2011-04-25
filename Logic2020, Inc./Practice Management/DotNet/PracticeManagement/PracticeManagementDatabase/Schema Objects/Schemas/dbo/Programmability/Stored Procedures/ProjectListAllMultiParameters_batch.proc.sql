-- exec dbo.ProjectListAllMultiParameters_batch @ClientIds=default,@ShowProjected=1,@ShowCompleted=1,@ShowActive=1,@ShowExperimental=0,@SalespersonIds=default,@PracticeManagerIds=default,@PracticeIds=default,@ProjectGroupIds=default
CREATE PROCEDURE dbo.ProjectListAllMultiParameters_batch  
 @ClientIds   VARCHAR(250) = NULL,  
 @ShowProjected  BIT = 0,  
 @ShowCompleted  BIT = 0,  
 @ShowActive   BIT = 0,  
 @ShowExperimental  BIT = 0,  
 @SalespersonIds  VARCHAR(250) = NULL,  
 @PracticeManagerIds VARCHAR(250) = NULL,  
 @PracticeIds   VARCHAR(250) = NULL,  
 @ProjectGroupIds  VARCHAR(250) = NULL  
AS   
 SET NOCOUNT ON ;  

--WITH sq AS 
--( SELECT f.ProjectId,  
--        dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,  
--        dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,  
--  
--        SUM(f.PersonMilestoneDailyAmount) AS Revenue,  
--  
--        SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,  
--  
--        ISNULL(SUM((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay), 0) AS Cogs,  
--  
--        SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) -  
--        ISNULL(SUM((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay), 0) AS GrossMargin,  
--  
--        ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,  
--  
--        SUM(f.PersonMilestoneDailyAmount - ISNULL(f.PayRate * f.PersonHoursPerDay, 0)) *  
--        (SELECT SUM(c.FractionOfMargin) FROM dbo.Commission AS c WHERE c.ProjectId = f.ProjectId AND c.CommissionType = 1) / 100 AS SalesCommission,  
--  
--        SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -  
--             (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * ISNULL(f.PersonHoursPerDay, 0)) *  
--            (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission  
--   FROM dbo.v_FinancialsRetrospective AS f  
--  WHERE f.Date BETWEEN @StartDate AND @EndDate  
--  GROUP BY f.ProjectId
--)   
 SELECT  p.ClientId,  
   p.ProjectId,  
   p.Discount,  
   p.Terms,  
   p.Name,  
   p.PracticeManagerId,  
   p.PracticeId,  
   p.StartDate,  
   p.EndDate,  
   p.ClientName,  
   p.PracticeName,  
   p.ProjectStatusId,  
   p.ProjectStatusName,  
   p.ProjectNumber,  
   p.BuyerName,  
   p.OpportunityId,  
   p.GroupId,  
        p.ClientIsChargeable,  
        p.ProjectIsChargeable,
 (SELECT mp.FirstName + ' ' + mp.LastName AS p
   FROM dbo.v_MilestonePerson AS mp  
WHERE mp.ProjectId = p.ProjectId
   FOR XML PATH('div'), TYPE) AS emp
 FROM dbo.v_Project AS p
 
 WHERE (  
     (  
    ( @ClientIds IS NULL OR  
     p.ClientId IN (  
     SELECT *  
     FROM   dbo.ConvertStringListIntoTable(@ClientIds) )  
    )  
    AND ( @ProjectGroupIds IS NULL OR  
     p.GroupId IN (  
     SELECT *  
     FROM   dbo.ConvertStringListIntoTable(@ProjectGroupIds) )  
     )  
   )  
   AND ( @PracticeIds IS NULL OR  
     p.PracticeId IN (  
       SELECT *  
       FROM   dbo.ConvertStringListIntoTable(@PracticeIds) )  
    )  
   AND ( ( @SalespersonIds IS NULL  
     AND @PracticeManagerIds IS NULL  
      )  
      OR EXISTS ( SELECT 1  
         FROM   dbo.v_PersonProjectCommission AS c  
         WHERE  c.ProjectId = p.ProjectId  
          AND c.PersonId IN (  
            SELECT *  
            FROM   dbo.ConvertStringListIntoTable(@SalespersonIds)  
          )  
          AND c.CommissionType = 1  
         UNION ALL  
         SELECT 1  
         FROM   dbo.v_PersonProjectCommission AS c  
         WHERE  c.ProjectId = p.ProjectId  
          AND c.PersonId IN (  
            SELECT *  
            FROM   dbo.ConvertStringListIntoTable(@PracticeManagerIds)  
          )  
          AND c.CommissionType = 2 )  
    )  
   )  
   AND ( ( @ShowProjected = 1  
     AND p.ProjectStatusId = 2  
      )  
      OR ( @ShowActive = 1  
        AND p.ProjectStatusId = 3  
      )  
      OR ( @ShowCompleted = 1  
        AND p.ProjectStatusId = 4  
      )  
      OR ( @ShowExperimental = 1  
        AND p.ProjectStatusId = 5  
      )  
    )  
 ORDER BY CASE p.ProjectStatusId  
      WHEN 2 THEN p.StartDate  
      ELSE p.EndDate  
    END  

