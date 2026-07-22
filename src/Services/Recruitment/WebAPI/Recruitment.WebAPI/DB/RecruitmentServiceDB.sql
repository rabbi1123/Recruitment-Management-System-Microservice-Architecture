-- =============================================================================
-- Recruitment Service Database (RecruitmentDB)
-- SQL Server
-- =============================================================================

IF DB_ID('RecruitmentDB') IS NULL
BEGIN
    CREATE DATABASE RecruitmentDB;
END
GO

USE RecruitmentDB;
GO

-- =============================================================================
-- Applications
-- =============================================================================
CREATE TABLE Applications
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    JobId BIGINT NOT NULL,
    CandidateId BIGINT NOT NULL,
    OrganizationId BIGINT NOT NULL,
    RecruiterId BIGINT NULL,

    CurrentStage NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Applications_CurrentStage DEFAULT ('Applied'),

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Applications_Status DEFAULT ('Active'),

    CoverLetter NVARCHAR(MAX) NULL,
    RejectionReason NVARCHAR(512) NULL,

    IsActive BIT NOT NULL,
    CreatedOn DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(450) NULL,
    UpdatedOn DATETIME2 NULL,
    UpdatedBy NVARCHAR(450) NULL,

    CONSTRAINT CK_Applications_CurrentStage CHECK
    (
        CurrentStage IN
        (
            'Applied',
            'UnderReview',
            'Shortlisted',
            'InterviewScheduled',
            'TechnicalInterview',
            'HRInterview',
            'OfferSent',
            'Hired',
            'Rejected'
        )
    ),

    CONSTRAINT CK_Applications_Status CHECK
    (
        Status IN
        (
            'Active',
            'Rejected',
            'Withdrawn',
            'Hired'
        )
    ),

    CONSTRAINT UQ_Applications_Job_Candidate
        UNIQUE (JobId, CandidateId)
);
GO

-- =============================================================================
-- Application Comments
-- =============================================================================
CREATE TABLE ApplicationComments
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    ApplicationId BIGINT NOT NULL,
    AuthorId BIGINT NOT NULL,

    Comment NVARCHAR(MAX) NOT NULL,

    IsActive BIT NOT NULL,
    CreatedOn DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(450) NULL,
    UpdatedOn DATETIME2 NULL,
    UpdatedBy NVARCHAR(450) NULL,

    CONSTRAINT FK_ApplicationComments_Applications
        FOREIGN KEY (ApplicationId)
        REFERENCES Applications(Id)
        ON DELETE CASCADE
);
GO

-- =============================================================================
-- Offers
-- =============================================================================
CREATE TABLE Offers
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    ApplicationId BIGINT NOT NULL,
    CandidateId BIGINT NOT NULL,
    JobId BIGINT NOT NULL,
    OrganizationId BIGINT NOT NULL,

    Position NVARCHAR(200) NOT NULL,

    Salary DECIMAL(12,2) NOT NULL,

    Currency NVARCHAR(3) NOT NULL
        CONSTRAINT DF_Offers_Currency DEFAULT ('USD'),

    JoiningDate DATE NULL,
    Benefits NVARCHAR(MAX) NULL,
    ExpirationDate DATE NULL,

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Offers_Status DEFAULT ('Draft'),

    SentDate DATETIME2 NULL,
    RespondedDate DATETIME2 NULL,

    IsActive BIT NOT NULL,
    CreatedOn DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(450) NULL,
    UpdatedOn DATETIME2 NULL,
    UpdatedBy NVARCHAR(450) NULL,

    CONSTRAINT CK_Offers_Status CHECK
    (
        Status IN
        (
            'Draft',
            'Sent',
            'Accepted',
            'Rejected',
            'Expired'
        )
    ),

    CONSTRAINT FK_Offers_Applications
        FOREIGN KEY (ApplicationId)
        REFERENCES Applications(Id)
        ON DELETE CASCADE
);
GO

-- =============================================================================
-- Indexes
-- =============================================================================

CREATE INDEX IX_Applications_JobId
ON Applications(JobId);
GO

CREATE INDEX IX_Applications_CandidateId
ON Applications(CandidateId);
GO

CREATE INDEX IX_Applications_OrganizationId
ON Applications(OrganizationId);
GO

CREATE INDEX IX_Applications_RecruiterId
ON Applications(RecruiterId);
GO

CREATE INDEX IX_Applications_CurrentStage
ON Applications(CurrentStage);
GO

CREATE INDEX IX_ApplicationComments_ApplicationId
ON ApplicationComments(ApplicationId);
GO

CREATE INDEX IX_Offers_ApplicationId
ON Offers(ApplicationId);
GO

CREATE INDEX IX_Offers_CandidateId
ON Offers(CandidateId);
GO

CREATE INDEX IX_Offers_JobId
ON Offers(JobId);
GO

CREATE INDEX IX_Offers_OrganizationId
ON Offers(OrganizationId);
GO

CREATE INDEX IX_Offers_Status
ON Offers(Status);
GO

-- =============================================================================
-- Create the following indexes after these tables are created
-- =============================================================================

-- CREATE INDEX IX_ApplicationStageHistory_ApplicationId
-- ON ApplicationStageHistory(ApplicationId);
-- GO

-- CREATE INDEX IX_Evaluations_ApplicationId
-- ON Evaluations(ApplicationId);
-- GO

-- CREATE INDEX IX_Evaluations_InterviewId
-- ON Evaluations(InterviewId);
-- GO