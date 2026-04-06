-- =====================================================
-- Student Management System - Table Creation Script
-- For SQL Server (Cloud/SQL Azure compatible)
-- =====================================================

-- =====================================================
-- Student Table Creation
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        -- Primary Key - Auto-increment ID
        Id INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Student Name (Required, max 100 characters)
        Name NVARCHAR(100) NOT NULL,
        
        -- Email Address (Required, unique constraint)
        Email NVARCHAR(100) NOT NULL UNIQUE,
        
        -- Age (Required, must be positive integer)
        Age INT NOT NULL CHECK (Age > 0),
        
        -- Course (Required, max 100 characters)
        Course NVARCHAR(100) NOT NULL,
        
        -- Created Date (Auto-set to current date/time)
        CreatedDate DATETIME2 DEFAULT GETDATE()
    );
    
    PRINT 'Table Students created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Students already exists.';
END
GO

-- =====================================================
-- Insert Sample Data (Optional - for testing)
-- =====================================================

-- Check if sample data already exists before inserting
IF NOT EXISTS (SELECT * FROM Students WHERE Email = 'john.doe@example.com')
BEGIN
    INSERT INTO Students (Name, Email, Age, Course, CreatedDate)
    VALUES 
        ('John Doe', 'john.doe@example.com', 22, 'Computer Science', GETDATE()),
        ('Jane Smith', 'jane.smith@example.com', 21, 'Information Technology', GETDATE()),
        ('Michael Johnson', 'michael.johnson@example.com', 23, 'Software Engineering', GETDATE());
    
    PRINT 'Sample data inserted successfully.';
END
GO

-- =====================================================
-- Create Index on Email for faster lookups
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Students_Email')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Students_Email ON Students(Email);
    PRINT 'Index on Email created successfully.';
END
GO