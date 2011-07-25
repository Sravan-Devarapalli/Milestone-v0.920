﻿CREATE TABLE [dbo].[Practice] (
    [PracticeId]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (100) NOT NULL,
    [PracticeManagerId] INT           NOT NULL,
    [IsActive]          BIT           NOT NULL,
    [IsCompanyInternal] BIT           NOT NULL,
	IsNotesRequired     BIT   CONSTRAINT Df_Practice_IsNotesRequired DEFAULT (1) NOT NULL
);


