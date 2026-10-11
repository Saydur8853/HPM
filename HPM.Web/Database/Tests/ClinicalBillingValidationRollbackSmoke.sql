SET NOCOUNT ON;
SET XACT_ABORT OFF;

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO Cash_Memo_DInvestigation
        (Patient_Name, Patient_Type, Total, Discount, Paid, Trans_Date, Module_ID, Entered_By)
    VALUES
        ('Validation Test', '1', 1000.00, 100.00, 600.00, GETDATE(), 11, 1);
    DECLARE @MemoID bigint = SCOPE_IDENTITY();

    INSERT INTO Cash_Memo_Sub_DInvestigation
        (Memo_ID, SLNo, Test_ID, Rate, Quantity)
    VALUES
        (@MemoID, 1, 1, 1000, 1);

    IF NOT EXISTS
    (
        SELECT 1
        FROM Cash_Memo_DInvestigation
        WHERE ID = @MemoID
          AND Total - Discount - Paid = 300.00
    )
        THROW 51000, 'Valid partial-payment memo was not persisted correctly.', 1;

    DECLARE @InvalidCashMemoRejected bit = 0;
    BEGIN TRY
        INSERT INTO Cash_Memo_DInvestigation
            (Patient_Name, Patient_Type, Total, Discount, Paid, Trans_Date, Module_ID, Entered_By)
        VALUES
            ('Invalid Discount', '1', 100.00, 101.00, 0.00, GETDATE(), 11, 1);
    END TRY
    BEGIN CATCH
        SET @InvalidCashMemoRejected = 1;
    END CATCH;

    IF @InvalidCashMemoRejected = 0
        THROW 51001, 'Cash memo amount constraint did not reject an excessive discount.', 1;

    DECLARE @InvalidOtMemoRejected bit = 0;
    BEGIN TRY
        INSERT INTO Cash_Memo_OT
            (Patient_Name, Patient_Type, Total, Discount, Entered_By, Trans_Date)
        VALUES
            ('Invalid OT Discount', '1', 100.00, -1.00, 1, GETDATE());
    END TRY
    BEGIN CATCH
        SET @InvalidOtMemoRejected = 1;
    END CATCH;

    IF @InvalidOtMemoRejected = 0
        THROW 51002, 'OT amount constraint did not reject a negative discount.', 1;

    ROLLBACK TRANSACTION;
    SELECT 'PASS' AS Result, CAST(300.00 AS decimal(18,2)) AS ExpectedDue;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
