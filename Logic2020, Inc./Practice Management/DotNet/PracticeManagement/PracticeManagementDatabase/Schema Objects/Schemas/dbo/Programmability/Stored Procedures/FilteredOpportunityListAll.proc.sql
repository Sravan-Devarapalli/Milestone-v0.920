CREATE PROCEDURE [dbo].[FilteredOpportunityListAll]
(
	@ShowActive			BIT = 1,
	@ShowExperimental	BIT = 0,
	@ShowInactive		BIT = 0,
	@ShowLost		BIT = 0,
	@ShowWon		BIT = 0,
	@ClientIds			VARCHAR(250) = NULL,
	@SalespersonIds		VARCHAR(250) = NULL,
	@OpportunityOwnerIds	VARCHAR(250) = NULL,
	@OpportunityGroupIds	VARCHAR(250) = NULL
	
)
AS
BEGIN

	-- Convert client ids from string to table
	DECLARE @ClientsList TABLE (Id INT)
	INSERT INTO @ClientsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert Opportunity group ids from string to table
	DECLARE @OpportunityGroupList TABLE (Id INT)
	INSERT INTO @OpportunityGroupList
	SELECT * FROM dbo.ConvertStringListIntoTable(@OpportunityGroupIds)

	-- Convert Opportunity owner ids from string to table
	DECLARE @OpportunityOwnersList TABLE (Id INT)
	INSERT INTO @OpportunityOwnersList
	SELECT * FROM dbo.ConvertStringListIntoTable(@OpportunityOwnerIds)

	-- Convert salesperson ids from string to table
	DECLARE @SalespersonsList TABLE (Id INT)
	INSERT INTO @SalespersonsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)


/*
		Go to issue #2432 for more details on default opportunities order
	*/
	;WITH CTE
	AS
	(
		
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') 
							ORDER BY CASE OP.sortOrder WHEN 0 THEN 1000 ELSE OP.sortOrder END,
									YEAR(ISNULL(O.ProjectedStartDate,dbo.GetFutureDate())),MONTH(ISNULL(O.ProjectedStartDate,dbo.GetFutureDate())),
									O.SalespersonLastName) RowNumber,
							/* here sortOrder = 0 means 'PO' priority */
			o.OpportunityId,
			o.Name,		   
			o.ClientId,
			o.SalespersonId,
			o.OpportunityStatusId,
			o.[Priority],
			o.PriorityId,
			op.sortOrder PrioritySortOrder,
			o.ProjectedStartDate,
			o.ProjectedEndDate,
			o.OpportunityNumber,
			o.Description,
			o.PracticeId,
			o.BuyerName,
			o.CreateDate,
			o.Pipeline,
			o.Proposed,
			o.SendOut,
			o.ClientName,
			o.SalespersonFirstName,
			o.SalespersonLastName,
			ps.Name AS SalespersonStatus,
			o.OpportunityStatusName,
			o.PracticeName,
			o.ProjectId,
			proj.ProjectNumber,
			o.OpportunityIndex,
			o.RevenueType,
			o.OwnerId,
			o.LastUpdate,
			o.GroupId,
			o.GroupName,
			o.PracticeManagerId,
			p.LastName as 'OwnerLastName',
			p.FirstName as 'OwnerFirstName',
			os.Name as 'OwnerStatus',
			o.EstimatedRevenue,
			o.OutSideResources
		FROM v_Opportunity O
		LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
		LEFT JOIN dbo.PersonStatus ps ON ps.PersonStatusId = o.SalespersonStatusId  
		LEFT JOIN dbo.PersonStatus os ON os.PersonStatusId = o.OwnerStatusId 
		LEFT JOIN dbo.Project proj ON proj.ProjectId = o.ProjectId
		INNER JOIN dbo.OpportunityPriorities AS op ON op.Id = o.PriorityId 
		WHERE
		/* ((@IsDiscussionReview2 = 1 AND o.OpportunityStatusId = 1)
			OR @IsDiscussionReview2 = 0 AND  (o.OpportunityStatusId = 1 OR @ActiveOnly = 0)
			)
			AND
			*/
			(@ClientIds IS NULL OR o.ClientId IN (SELECT * FROM @ClientsList))
			AND (o.SalespersonId IN (SELECT * FROM @SalespersonsList) OR @SalespersonIds IS NULL)
			AND (o.OwnerId IN (SELECT * FROM @OpportunityOwnersList) OR @OpportunityOwnerIds IS NULL)
			AND (o.GroupId IN (SELECT * FROM @OpportunityGroupList) OR @OpportunityGroupIds IS NULL)
			/* AND (o.Name LIKE @Looked OR o.Description LIKE @Looked OR o.ClientName LIKE @Looked OR o.OpportunityNumber LIKE @Looked OR o.BuyerName LIKE @Looked)	*/
			/*AND (o.OpportunityId in (select ot.OpportunityId from OpportunityTransition ot where ot.TargetPersonId = @TargetPersonId) OR @TargetPersonId IS NULL)*/
			AND (    
						(@ShowActive = 1 AND o.OpportunityStatusId = 1 )
				    OR  (@ShowLost = 1 AND o.OpportunityStatusId = 2 )
				    OR  (@ShowInactive = 1 AND o.OpportunityStatusId = 3 ) 
				    OR  (@ShowWon = 1 AND o.OpportunityStatusId = 4 )
				    OR  (@ShowExperimental = 1 AND o.OpportunityStatusId = 5 )
				 
			    )

				/*
				OpportunityStatusId	Name
								1	Active
								2	Lost
								3	Inactive
								4	Won
								5	Experimental
				*/
		)
		
		SELECT 
			B.*
		FROM CTE A
		JOIN CTE B	ON (A.ClientName =B.ClientName AND isnull(A.BuyerName, '')  = isnull(B.BuyerName, '') 
							AND A.RowNumber=1 AND A.PrioritySortOrder!=0 AND B.PrioritySortOrder != 0 ) 
					   OR (A.OpportunityId = B.OpportunityId AND A.PrioritySortOrder=0)
		ORDER BY A.PrioritySortOrder,
				YEAR(ISNULL(A.ProjectedStartDate,dbo.GetFutureDate())),
				MONTH(ISNULL(A.ProjectedStartDate,dbo.GetFutureDate())),
				A.SalespersonLastName,
				B.ClientName,
				ISNULL(B.BuyerName, ''),
				B.PrioritySortOrder,
				B.SalespersonLastName

		/* here sortOrder = 0 means 'PO' priority */

END
	
