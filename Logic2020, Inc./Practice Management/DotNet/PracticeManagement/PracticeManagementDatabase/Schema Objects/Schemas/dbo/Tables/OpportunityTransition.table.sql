CREATE TABLE [dbo].[OpportunityTransition] (
    [OpportunityTransitionId]       INT             IDENTITY (1, 1) NOT NULL,
    [OpportunityId]                 INT             NOT NULL,
    [OpportunityTransitionStatusId] INT             NOT NULL,
    [TransitionDate]                DATETIME        NOT NULL,
    [PersonId]                      INT             NOT NULL,
    [NoteText]                      NVARCHAR (2000) NULL,
    [OpportunityTransitionTypeId]   INT             NOT NULL,
    [TargetPersonId]                INT             NULL
);


