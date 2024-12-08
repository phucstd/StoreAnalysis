-- Enable IDENTITY_INSERT for the Slots table
SET IDENTITY_INSERT Slots ON;

-- Insert slots with explicit SlotID values
INSERT INTO Slots (SlotID, Name, IsEmpty, LastRefillDate)
VALUES
    (1, 'A1', 0, '2024-12-01'),
    (2, 'A2', 0, '2024-12-01'),
    (3, 'A3', 0, '2024-12-01'),
    (4, 'A4', 0, '2024-12-01'),
    (5, 'B1', 0, '2024-12-01'),
    (6, 'B2', 0, '2024-12-01'),
    (7, 'B3', 0, '2024-12-01'),
    (8, 'B4', 0, '2024-12-01'),
    (9, 'C1', 0, '2024-12-01'),
    (10, 'C2', 0, '2024-12-01'),
    (11, 'C3', 0, '2024-12-01'),
    (12, 'C4', 0, '2024-12-01');

-- Disable IDENTITY_INSERT for the Slots table
SET IDENTITY_INSERT Slots OFF;
