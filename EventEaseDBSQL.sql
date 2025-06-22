-- database creation
USE master




CREATE DATABASE EventEaseDB

USE EventEaseDB

-- TABLE CREATION
CREATE TABLE Venue(
VenueID INT IDENTITY(100,1) PRIMARY KEY NOT NULL,
VenueName VARCHAR(250) NOT NULL,
[Location] VARCHAR(250) NOT NULL,
Capacity INT NOT NULL,
ImageUrl VARCHAR(250) NOT NULL

);

-- question 3 
CREATE TABLE EventType (
EventTypeID INT IDENTITY(5000,1) PRIMARY KEY,
Name VARCHAR(250) NOT NULL
);

-- TABLE CREATION
CREATE TABLE [Event](
EventID INT IDENTITY(200,1) PRIMARY KEY NOT NULL,
VenueID INT NOT NULL,
EventTypeID INT NULL, 
EventName VARCHAR(250) NOT NULL,
EventDate DATE NOT NULL,
[Description] VARCHAR(250) NOT NULL,
FOREIGN KEY (VenueID) REFERENCES Venue(VenueID), 
FOREIGN KEY (EventTypeID) references EventType(EventTypeID) ON DELETE SET NULL
);

-- TABLE CREATION
CREATE TABLE Booking(
BookingID INT IDENTITY(300,1) PRIMARY KEY NOT NULL,
EventID INT NOT NULL,
VenueID INT NOT NULL,
BookingDate DATE NOT NULL,
FOREIGN KEY (EventID) REFERENCES [Event](EventID) ON DELETE CASCADE,
FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE,
-- CONSTRAINT UQ_VENUE_EVENT UNIQUE (VenueID, EventID) -- no double boook of same venue for same event 
);


--Table Insertion
-- INSERT: Venue Data
INSERT INTO Venue (VenueName, [Location], Capacity, ImageUrl) 
VALUES 
('Farmyard Party Venue', 'Honeydew, Johannesburg', 100, 'https://lh3.googleusercontent.com/gps-cs-s/AB5caB-4qDVJBScHhVrD1ipcfXi2Y6sKoJ2FAArio7tNrfDsgqwNGUSk0K-Gb-ReysOiXi4cwdrLt3Oka77FsY_o-Vd2_vCwb0C1KwB'),
('Très Jolie', 'Peter Rd, Muldersdrift', 350, 'https://images.app.goo.gl/Ck4a2LoboRJ7WP6c7'),
('Blueberry Hill Hotel', 'Honeydew, Roodepoort', 200, 'https://images.app.goo.gl/vsDW5AWbP1G6TNPd6'),
('The Venue Melrose Arch', 'Melrose, Johannesburg', 420, 'https://images.app.goo.gl/FEbMw1iqFs3vbBYB6'),
('Loftus Versfeld Stadium', 'Arcadia, Pretoria', 51762, 'https://images.app.goo.gl/PmH29NidCndENU2x7');



-- Table Insertion
-- INSERT: Event Type Data
INSERT INTO EventType (Name) 
VALUES 
('Conference'),
('Wedding'),
('Naming'),
('Birthday'),
('Concert');


--Table Insertion
-- INSERT: Event Data
INSERT INTO [Event] (VenueID, EventName, EventDate, [Description], EventTypeID) 
VALUES 
(100, 'Spring Wedding', '2025-09-21', 'A beautiful garden wedding ceremony.', 5000),
(101, 'Jazz Night', '2025-10-05', 'Live jazz band and wine tasting.', 5001),
(102, 'Tech Conference 2025', '2025-11-15', 'Annual tech innovations and networking.' , 5002),
(103, 'Fashion Gala', '2025-12-02', 'Runway event with SA designers.', 5003),
(104, 'Football Final', '2025-12-20', 'Premier league season final.', 5004);

--Table Insertion
-- INSERT: Booking Data

INSERT INTO Booking (EventID, VenueID, BookingDate) 
VALUES 
(200, 100, '2025-07-10'),
(201, 101, '2025-08-01'),
(202, 102, '2025-08-15'),
(203, 103, '2025-09-01'),
(204, 104, '2025-10-10');




--TABLE MANIPULATION
-- SELECT ALL TABLES (Preview)
--SELECT * FROM Venue;
--SELECT * FROM [Event];
--SELECT * FROM Booking;

CREATE VIEW BookingOverviewView AS
SELECT 
    b.BookingID,
    b.BookingDate,
    e.EventID,
    e.EventName,
    e.EventDate,
    e.Description AS EventDescription,
    v.VenueID,
    v.VenueName,
    v.Location AS VenueLocation,
    v.Capacity,
    v.ImageUrl
FROM 
    Booking b
JOIN 
    [Event] e ON b.EventID = e.EventID
JOIN 
    Venue v ON b.VenueID = v.VenueID;

	
	