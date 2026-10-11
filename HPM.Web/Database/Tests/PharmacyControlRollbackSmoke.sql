SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @StoreID int = (SELECT TOP (1) ID FROM dbo.Medicine_Store ORDER BY ID);
    IF @StoreID IS NULL THROW 51100, 'Medicine store seed is required.', 1;

    INSERT dbo.Service(Name, Sale_Rate, Buy_Rate, Category_ID)
    VALUES(N'Pharmacy Control Smoke Medicine', 20, 10, 3);
    DECLARE @ServiceID int = SCOPE_IDENTITY();

    INSERT dbo.Medicine_Store_Balance(Service_ID, Medicine_Store_ID, Balance, Expire_Date, Batch_No)
    VALUES(@ServiceID, @StoreID, 10, DATEADD(year, 1, GETDATE()), N'SMOKE-BATCH');
    DECLARE @StockID int = SCOPE_IDENTITY();

    INSERT dbo.Prescription(Name, Visit_No, Visit_Date, Type, Referred_By)
    VALUES(N'Smoke Patient', 1, GETDATE(), N'OPD', 1);
    DECLARE @PrescriptionID bigint = SCOPE_IDENTITY();

    INSERT dbo.Prescription_Medicine(Prescription_ID, Service_ID, Interval, Dose, Days, Quantity, SLNO)
    VALUES(@PrescriptionID, @ServiceID, 2, 1, 5, 10, 1);
    DECLARE @PrescriptionMedicineID int = SCOPE_IDENTITY();

    INSERT dbo.Cash_Memo(Name, Patient_Type, Total, Discount, Entered_By, Trans_Date, Is_Package, Is_Credit, Buffer_Store)
    VALUES(N'Smoke Patient', N'1', 40, 0, 1, GETDATE(), 0, 0, @StoreID);
    DECLARE @MemoID bigint = SCOPE_IDENTITY();

    INSERT dbo.Cash_Memo_Sub(Bill_No, Service_ID, SLNO, Rate, Quantity, Buy_Rate)
    VALUES(@MemoID, @ServiceID, 1, 20, 2, 10);
    DECLARE @MemoSubID bigint = SCOPE_IDENTITY();

    INSERT dbo.Pharmacy_Prescription_Dispense(Prescription_ID, Cash_Memo_ID, Entered_By, Trans_Date)
    VALUES(@PrescriptionID, @MemoID, 1, GETDATE());
    DECLARE @DispenseID bigint = SCOPE_IDENTITY();
    INSERT dbo.Pharmacy_Prescription_Dispense_Sub(Dispense_ID, Prescription_Medicine_ID, Service_ID, Quantity)
    VALUES(@DispenseID, @PrescriptionMedicineID, @ServiceID, 2);

    INSERT dbo.Pharmacy_Sales_Return(Cash_Memo_ID, Store_ID, Refund_Amount, Reason, Is_Posted, Entered_By, Trans_Date)
    VALUES(@MemoID, @StoreID, 20, N'Smoke return', 1, 1, GETDATE());
    DECLARE @SalesReturnID bigint = SCOPE_IDENTITY();
    INSERT dbo.Pharmacy_Sales_Return_Sub(Sales_Return_ID, Cash_Memo_Sub_ID, Service_ID, Stock_Balance_ID, Quantity, Rate, Buy_Rate, Is_Restocked)
    VALUES(@SalesReturnID, @MemoSubID, @ServiceID, @StockID, 1, 20, 10, 1);

    INSERT dbo.Medicine_Stock_Adjustment(Store_ID, Adjustment_Type, Reason, Is_Posted, Entered_By, Trans_Date)
    VALUES(@StoreID, N'Damaged Disposal', N'Smoke disposal', 1, 1, GETDATE());
    DECLARE @AdjustmentID bigint = SCOPE_IDENTITY();
    INSERT dbo.Medicine_Stock_Adjustment_Sub(Adjustment_ID, Stock_Balance_ID, Service_ID, Batch_No, Expire_Date, Before_Quantity, Change_Quantity, After_Quantity, Unit_Cost)
    VALUES(@AdjustmentID, @StockID, @ServiceID, N'SMOKE-BATCH', DATEADD(year, 1, GETDATE()), 10, -1, 9, 10);

    IF NOT EXISTS (SELECT 1 FROM dbo.Pharmacy_Prescription_Dispense_Sub WHERE Dispense_ID=@DispenseID)
        THROW 51101, 'Prescription dispense detail was not persisted.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Pharmacy_Sales_Return_Sub WHERE Sales_Return_ID=@SalesReturnID)
        THROW 51102, 'Sales return detail was not persisted.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Medicine_Stock_Adjustment_Sub WHERE Adjustment_ID=@AdjustmentID)
        THROW 51103, 'Stock adjustment detail was not persisted.', 1;

    ROLLBACK TRANSACTION;
    PRINT 'PASS: prescription dispense, sales return and stock adjustment rollback smoke test.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
