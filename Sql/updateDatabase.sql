-- Use the TEST_DOO database
USE TEST_DOO;

-- Change username of users
UPDATE HR.Employees
SET username = firstname;

-- Add a new column named 'quantity' to the table
ALTER TABLE Production.Products
ADD stock decimal(18, 2);
GO

-- Update the 'quantity' column with random numbers between 0.0 and 1000.0
UPDATE Production.Products
SET stock = 
  CASE 
    WHEN RAND(CHECKSUM(NEWID())) < 0.2 THEN 0.0
    ELSE CAST(ROUND(RAND(CHECKSUM(NEWID())) * 1000, 0) AS FLOAT)
  END;