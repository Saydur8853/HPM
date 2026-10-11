SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @Now datetime = GETDATE();
    DECLARE @GenericID int;
    DECLARE @TypeID int;
    DECLARE @SupplierID int;
    DECLARE @ServiceID int;

    INSERT INTO Generic_Group (Name, Entered_By, Trans_Date) VALUES (N'UAT Generic', 1, @Now);
    SET @GenericID = SCOPE_IDENTITY();
    INSERT INTO Medicine_Type (Type, Entered_By, Trans_Date) VALUES (N'UAT Tablet', 1, @Now);
    SET @TypeID = SCOPE_IDENTITY();
    INSERT INTO Supplier_Pharmacy (Name, Address, Contact, Amount, Trans_Date)
    VALUES (N'UAT Supplier', N'UAT Address', N'01000000000', 0, @Now);
    SET @SupplierID = SCOPE_IDENTITY();

    INSERT INTO Service (Name, Sale_Rate, Buy_Rate, Re_Order_Level, Is_VAT_App, Category_ID, Type_ID, Generic_ID, Supplier_ID, Investigation_Module_ID)
    VALUES (N'UAT Medicine 500mg', 12, 10, 20, 0, 3, @TypeID, @GenericID, @SupplierID, 0);
    SET @ServiceID = SCOPE_IDENTITY();

    INSERT INTO Medicine_Store_Balance (Service_ID, Medicine_Store_ID, Balance, Expire_Date, Batch_No)
    VALUES (@ServiceID, 1, 100, DATEADD(year, 1, CAST(@Now AS date)), N'UAT-BATCH');

    IF NOT EXISTS (
        SELECT 1 FROM Service s
        JOIN Generic_Group g ON g.ID = s.Generic_ID
        JOIN Medicine_Type t ON t.ID = s.Type_ID
        JOIN Supplier_Pharmacy p ON p.ID = s.Supplier_ID
        JOIN Medicine_Store_Balance b ON b.Service_ID = s.ID
        WHERE s.ID = @ServiceID AND s.Category_ID = 3 AND b.Balance = 100)
        THROW 51000, 'Pharmacy master linkage invariant failed.', 1;

    SELECT N'PASS' AS Result, @ServiceID AS Medicine_ID, @SupplierID AS Supplier_ID;
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

IF EXISTS (SELECT 1 FROM Service WHERE Name = N'UAT Medicine 500mg')
    THROW 51000, 'Rollback verification failed.', 1;

SELECT N'PASS' AS Rollback_Result;
