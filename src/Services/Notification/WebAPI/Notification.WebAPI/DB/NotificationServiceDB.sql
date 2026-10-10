IF DB_ID('NotificationDB') IS NULL
BEGIN
    CREATE DATABASE NotificationDB;
END
GO

USE NotificationDB;
GO


/* =========================================================
   Notifications
   ========================================================= */

CREATE TABLE Notifications
(
    Id BIGINT IDENTITY(1,1) NOT NULL,

    -- Integration event ID from RabbitMQ
    EventId UNIQUEIDENTIFIER NOT NULL,

    -- User who receives the notification
    UserId BIGINT NULL,

    -- EMAIL / SMS / PUSH / IN_APP
    Channel VARCHAR(20) NOT NULL,

    -- Email address / phone number / device token
    Recipient NVARCHAR(500) NOT NULL,

    -- Example:
    -- EmailVerification
    -- PasswordReset
    -- InterviewScheduled
    TemplateName NVARCHAR(200) NULL,

    -- Template version used to generate this notification
    TemplateVersion INT NULL,

    -- Used mainly for Push notifications
    Title NVARCHAR(500) NULL,

    -- Used mainly for Email notifications
    Subject NVARCHAR(500) NULL,

    -- Rendered notification content
    Body NVARCHAR(MAX) NULL,

    -- PENDING / PROCESSING / SENT / DELIVERED /
    -- FAILED / RETRYING / CANCELLED
    Status VARCHAR(30) NOT NULL,

    -- Number of attempts
    RetryCount INT NOT NULL
        CONSTRAINT DF_Notifications_RetryCount
        DEFAULT 0,

    -- SMTP / SendGrid / Amazon SES / Twilio / Firebase etc.
    Provider NVARCHAR(100) NULL,

    -- ID returned by external notification provider
    ProviderMessageId NVARCHAR(500) NULL,

    -- Provider error information
    ErrorCode NVARCHAR(100) NULL,

    ErrorMessage NVARCHAR(MAX) NULL,

    -- Used when notification is scheduled
    ScheduledOn DATETIME2(3) NULL,

    -- When notification was sent
    SentOn DATETIME2(3) NULL,

    -- When provider confirmed delivery
    DeliveredOn DATETIME2(3) NULL,

    -- Audit fields
    CreatedOn DATETIME2(3) NOT NULL
        CONSTRAINT DF_Notifications_CreatedOn
        DEFAULT SYSUTCDATETIME(),

    CreatedBy NVARCHAR(450) NULL,

    UpdatedOn DATETIME2(3) NULL,

    UpdatedBy NVARCHAR(450) NULL,

    CONSTRAINT PK_Notifications
        PRIMARY KEY (Id),

    CONSTRAINT CK_Notifications_Channel
        CHECK
        (
            Channel IN
            (
                'EMAIL',
                'SMS',
                'PUSH',
                'IN_APP'
            )
        ),

    CONSTRAINT CK_Notifications_Status
        CHECK
        (
            Status IN
            (
                'PENDING',
                'PROCESSING',
                'SENT',
                'DELIVERED',
                'FAILED',
                'RETRYING',
                'CANCELLED'
            )
        )
);
GO


/* =========================================================
   Indexes
   ========================================================= */

CREATE UNIQUE INDEX UX_Notifications_EventId_Channel
ON Notifications(EventId, Channel);
GO

CREATE INDEX IX_Notifications_UserId
ON Notifications(UserId);
GO

CREATE INDEX IX_Notifications_Status_CreatedOn
ON Notifications(Status, CreatedOn);
GO

CREATE INDEX IX_Notifications_Channel_CreatedOn
ON Notifications(Channel, CreatedOn);
GO

CREATE INDEX IX_Notifications_Recipient
ON Notifications(Recipient);
GO

CREATE INDEX IX_Notifications_ProviderMessageId
ON Notifications(ProviderMessageId)
WHERE ProviderMessageId IS NOT NULL;
GO