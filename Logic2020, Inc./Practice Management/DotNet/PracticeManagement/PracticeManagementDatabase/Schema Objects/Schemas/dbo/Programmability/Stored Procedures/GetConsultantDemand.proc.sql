CREATE PROCEDURE dbo.GetConsultantDemand
(
	@StartDate DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN 
	SELECT 
		O.OpportunityId ObjectId,
		O.Name [ObjectName],
		O.OpportunityNumber [ObjectNumber],
		O.PriorityId [ObjectStatusId],
		C.ClientId,
		C.Name ClientName,
		P.PersonId,
		P.LastName,
		P.FirstName,
		dbo.GetDailyDemand(@StartDate, @EndDate, P.PersonId, O.OpportunityId, 1) QuantityString ,-- CONVERT(NVARCHAR,OP.Quantity)
		CONVERT(INT,1) ObjectType,
		O.ProjectedStartDate [StartDate],
		ISNULL(O.ProjectedEndDate, dbo.GetFutureDate()) [EndDate],
		NULL [LinkedObjectId],
		NULL [LinkedObjectNumber]
		
	FROM dbo.OpportunityPersons OP
	JOIN dbo.Opportunity O ON O.OpportunityId = OP.OpportunityId
	JOIN dbo.Person P ON P.PersonId = OP.PersonId
	JOIN dbo.Client C ON O.ClientId = C.ClientId 
	WHERE OP.RelationTypeId = 2 -- Team Structure
		AND OP.NeedBy <= @EndDate AND OP.NeedBy >= @StartDate
		AND O.ProjectedStartDate <= @EndDate AND ISNULL(O.ProjectedEndDate, dbo.GetFutureDate()) >= @StartDate
		AND O.PriorityId IN (1, 2) AND O.OpportunityStatusId = 1 --Priorities A OR B with Active Status.
		AND O.ProjectId IS NULL --Only Opportunities Not Linked To Project.

	UNION

	SELECT P.ProjectId ObjectId, 
			P.Name [ObjectName], 
			P.ProjectNumber [ObjectNumber], 
			P.ProjectStatusId [ObjectStatusId],
			P.ClientId, 
			C.Name ClientName, 
			Per.PersonId, 
			Per.LastName, 
			Per.FirstName, 
			dbo.GetDailyDemand(@StartDate, @EndDate, Per.PersonId, P.ProjectId, 2) QuantityString, -- COUNT(MPE.MilestoneId)
			CONVERT(INT,2) ObjectType,
			P.StartDate [StartDate],
			P.EndDate [EndDate],
			P.OpportunityId [LinkedObjectId],
			O.OpportunityNumber [LinkedObjectNumber]
	FROM Project P
	JOIN Client C ON C.ClientId = P.ClientId
	JOIN Milestone M ON M.ProjectId = P.ProjectId
	JOIN MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	JOIN MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	JOIN Person Per ON Per.PersonId = MP.PersonId
	LEFT JOIN Opportunity O ON O.OpportunityId = P.OpportunityId
	WHERE Per.IsStrawman = 1 
		AND MPE.StartDate <= @EndDate AND MPE.StartDate >= @StartDate
		AND P.StartDate <= @EndDate AND P.EndDate >= @StartDate
		AND P.ProjectStatusId IN (2,3) -- Only Active and Projected status Projects.
	GROUP BY P.ProjectId, Per.PersonId,  Per.LastName, Per.FirstName, P.ProjectStatusId, P.Name, P.ProjectNumber, P.ClientId, C.Name, P.StartDate, P.EndDate, P.OpportunityId, O.OpportunityNumber
END
