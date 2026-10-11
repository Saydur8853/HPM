SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @SupplierID int = (SELECT TOP (1) ID FROM dbo.Supplier_Pharmacy ORDER BY ID);
    DECLARE @StoreID int = (SELECT TOP (1) ID FROM dbo.Medicine_Store ORDER BY ID);
    DECLARE @ServiceID int = (SELECT TOP (1) ID FROM dbo.Service WHERE Category_ID = 3 ORDER BY ID);

    IF @SupplierID IS NULL
    BEGIN
        INSERT dbo.Supplier_Pharmacy(Name, Address, Contact, Amount, Trans_Date)
        VALUES(N'Smoke Test Supplier', N'Test', N'000', 0, GETDATE());
        SET @SupplierID = SCOPE_IDENTITY();
    END

    IF @ServiceID IS NULL
    BEGIN
        INSERT dbo.Service(Name, Sale_Rate, Buy_Rate, Category_ID)
        VALUES(N'Smoke Test Medicine', 12, 10, 3);
        SET @ServiceID = SCOPE_IDENTITY();
    END

    IF @StoreID IS NULL
        THROW 51000, 'Medicine store seed is required for this smoke test.', 1;

    INSERT dbo.Medicine_Return(Supplier_ID, Store_ID, Is_Posted, Trans_Date, Entered_By)
    VALUES(@SupplierID, @StoreID, 1, GETDATE(), 1);
    DECLARE @ReturnID int = SCOPE_IDENTITY();

    INSERT dbo.Medicine_Return_Sub(Medicine_Return_ID, Service_ID, SLNO, Quantity, Buy_Rate)
    VALUES(@ReturnID, @ServiceID, 1, 1, 10);

    INSERT dbo.Supplier_Payment(Supplier_ID, Amount, Payment_Mode, Entered_By, Trans_Date)
    VALUES(@SupplierID, 10, N'Cash', 1, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM dbo.Medicine_Return_Sub WHERE Medicine_Return_ID = @ReturnID)
        THROW 51001, 'Medicine return detail was not persisted.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Supplier_Payment WHERE Supplier_ID = @SupplierID AND Amount = 10)
        THROW 51002, 'Supplier payment was not persisted.', 1;

    ROLLBACK TRANSACTION;
    PRINT 'PASS: pharmacy return/payment insert and rollback smoke test.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
