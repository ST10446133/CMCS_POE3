USE POE3_CMCS
GO

-- Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);
GO

-- Sample users
INSERT INTO Users (Username, Password, Role)
VALUES
('lecturer1', '12345', 'Lecturer'),
('coordinator1', 'admin123', 'Coordinator'),
('manager1', 'managerpass', 'Manager');
GO

-- Claims table
CREATE TABLE Claims (
    ClaimId INT IDENTITY(1,1) PRIMARY KEY,
    LecturerUsername NVARCHAR(100) NOT NULL,
    HoursWorked DECIMAL(10,2) NOT NULL,
    HourlyRate DECIMAL(10,2) NOT NULL,
    Notes NVARCHAR(500) NULL,
    [Supporting Doc] VARBINARY(MAX) NULL,
    FileName NVARCHAR(255) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    DateSubmitted DATETIME NOT NULL DEFAULT GETDATE()
);
GO

ALTER TABLE Claims 
ADD TotalAmount DECIMAL (18,2) NOT NULL DEFAULT 0;

ALTER TABLE Claims
ADD VerificationNotes NVARCHAR(MAX),
VerifiedBy NVARCHAR(100),
VerifiedAt DATETIME NULL,
FinalApprovedBy NVARCHAR(100),
FinalApprovedAt DATETIME NULL;
