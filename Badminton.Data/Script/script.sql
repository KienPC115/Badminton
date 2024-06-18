USE master ;  
GO  
-- Create the database
CREATE DATABASE Net1710_221_8_Badminton;
GO

-- Use the newly created database
USE Net1710_221_8_Badminton;
GO

-- Create the User table
CREATE TABLE [Customer] (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Phone VARCHAR(50) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Address NVARCHAR(100) NOT NULL,
	Email VARCHAR(50) NOT NULL,
	DateOfBirth DATETIME NOT NULL
);
GO

-- Create the Court table
CREATE TABLE Court (
    CourtId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(20) NOT NULL,
    Status VARCHAR(20) NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Price FLOAT NOT NULL
);
GO

-- Create the Order table
CREATE TABLE [Order] (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    Type VARCHAR(50) NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    OrderNotes VARCHAR(500),
    TotalAmount FLOAT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES [Customer](CustomerId)
);
GO
-- Create the order detail table
CREATE TABLE CourtDetail(
	CourtDetailId INT IDENTITY(1,1) PRIMARY KEY,
	CourtId INT NOT NULL,
	Slot VARCHAR(50) NOT NULL,
	Price FLOAT NOT NULL,
	Status VARCHAR(50),
	FOREIGN KEY (CourtId) REFERENCES Court(CourtId)
)
GO
-- Create the OrderDetail table
CREATE TABLE OrderDetail (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
	CourtDetailId INT NOT NULL,
	Amount FLOAT NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
    FOREIGN KEY (CourtDetailId) REFERENCES CourtDetail(CourtDetailId),
);
GO

USE Net1710_221_8_Badminton;
GO

-- Delete all records from the tables
DELETE FROM OrderDetail;
DELETE FROM CourtDetail;
DELETE FROM [Order];
DELETE FROM Court;
DELETE FROM [Customer];
GO


-- Insert dummy data into Customer table
INSERT INTO [Customer] (Phone, Name, Address, Email, DateOfBirth) VALUES
('1234567890', N'John Doe', N'123 Main St', 'john.doe@example.com', '1985-06-15'),
('2345678901', N'Jane Smith', N'456 Elm St', 'jane.smith@example.com', '1990-07-20'),
('3456789012', N'Michael Johnson', N'789 Oak St', 'michael.johnson@example.com', '1978-08-25'),
('4567890123', N'Emily Davis', N'101 Pine St', 'emily.davis@example.com', '1992-09-30'),
('5678901234', N'Chris Lee', N'202 Maple St', 'chris.lee@example.com', '1983-10-05'),
('6789012345', N'Sarah Brown', N'303 Cedar St', 'sarah.brown@example.com', '1987-11-10');
GO

-- Insert dummy data into Court table
INSERT INTO Court (Name, Status, Description, Price) VALUES
('Court 1', 'Available', N'Standard court', 50.0),
('Court 2', 'Booked', N'Premium court', 75.0),
('Court 3', 'Available', N'Outdoor court', 40.0),
('Court 4', 'Available', N'Indoor court', 65.0),
('Court 5', 'Available', N'Standard court', 50.0),
('Court 6', 'Available', N'Outdoor court', 45.0);
GO

-- Insert dummy data into Order table
INSERT INTO [Order] (CustomerId, Type, TotalAmount, OrderDate, OrderNotes) VALUES
(1, 'Direct Payment', 100.0, '2024-05-01 00:00:00.000', 'Contrary to popular belief, Lorem Ipsum is not simply random text'),
(2, 'Direct Payment', 150.0, '2024-05-01 00:00:00.000', 'There are many variations of passages of Lorem Ipsum available.'),
(3, 'Direct Payment', 300.0, '2024-05-01 00:00:00.000', 'Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of "de Finibus Bonorum et Malorum" (The Extremes of Good and Evil) by Cicero'),
(4, 'Direct Payment', 250.0, '2024-05-01 00:00:00.000', 'The first line of Lorem Ipsum, "Lorem ipsum dolor sit amet..", comes from a line in section 1.10.32.'),
(5, 'Direct Payment', 75.0, '2024-05-01 00:00:00.000', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam'),
(6, 'Direct Payment', 200.0, '2024-05-01 00:00:00.000', 'Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. ');
GO

-- Insert dummy data into CourtDetail table
INSERT INTO CourtDetail (CourtId, Slot, Price, Status) VALUES
(1, '08h00-9h30', 50.0, 'Available'),
(2, '09h30-11h00', 75.0, 'Available'),
(3, '12h00-13h30', 40.0, 'Available'),
(4, '13h30-15h00', 65.0, 'Available'),
(5, '15h00-16h30', 50.0, 'Available'),
(6, '16h30-18h00', 45.0, 'Available'),
(1, '18h00-19h30', 50.0, 'Available'),
(2, '19h30-21h00', 75.0, 'Available'),
(3, '21h00-22h30', 40.0, 'Available');
GO

-- Insert dummy data into OrderDetail table
INSERT INTO OrderDetail (OrderId, CourtDetailId, Amount) VALUES
(1, 1, 50.0),
(2, 2, 75.0),
(3, 3, 40.0),
(4, 4, 65.0),
(5, 5, 50.0),
(6, 6, 45.0);
GO

