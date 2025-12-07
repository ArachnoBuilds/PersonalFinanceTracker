SELECT c.Description, b.Amount, b.Id
FROM Category c JOIN Budget b ON c.Id = b.Category_Id
WHERE b.Year = 2025 AND b.Month = 1 AND c.Type = "Expenses"