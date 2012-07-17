CREATE PROCEDURE dbo.GetConsultantDemand
(
	@StartDate DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN 

    DECLARE @FutureDate DATETIME 
	SET @FutureDate = dbo.GetFutureDate()

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
		1 ObjectType,
		O.ProjectedStartDate [StartDate],
		ISNULL(O.ProjectedEndDate, @FutureDate) [EndDate],
		NULL [LinkedObjectId],
		NULL [LinkedObjectNumber],
		O.Description AS OpportunintyDescription,
		'' AS ProjectDescription 
	FROM dbo.OpportunityPersons OP
	INNER JOIN dbo.Opportunity O ON O.OpportunityId = OP.OpportunityId
	INNER JOIN dbo.Person P ON P.PersonId = OP.PersonId
	INNER JOIN dbo.Client C ON O.ClientId = C.ClientId 
	WHERE OP.RelationTypeId = 2 -- Team Structure
		AND OP.NeedBy <= @EndDate AND OP.NeedBy >= @StartDate
		AND O.ProjectedStartDate <= @EndDate AND ISNULL(O.ProjectedEndDate, @FutureDate) >= @StartDate
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
			2 ObjectType,
			P.StartDate [StartDate],
			P.EndDate [EndDate],
			P.OpportunityId [LinkedObjectId],
			O.OpportunityNumber [LinkedObjectNumber],
			O.Description AS OpportunintyDescription,
			P.Description AS ProjectDescription 
	FROM dbo.Project P
	INNER JOIN dbo.Client C ON C.ClientId = P.ClientId
	INNER JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
	INNER JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	LEFT JOIN dbo.Opportunity O ON O.OpportunityId = P.OpportunityId
	WHERE Per.IsStrawman = 1 
		AND MPE.StartDate <= @EndDate AND MPE.StartDate >= @StartDate
		AND P.StartDate <= @EndDate AND P.EndDate >= @StartDate
		AND P.ProjectStatusId IN (2,3) -- Only Active and Projected status Projects.
	GROUP BY P.ProjectId, Per.PersonId,  Per.LastName, Per.FirstName, P.ProjectStatusId, P.Name, P.ProjectNumber, P.ClientId, C.Name, P.StartDate, P.EndDate, P.OpportunityId, O.OpportunityNumber,P.Description,O.Description 
END

