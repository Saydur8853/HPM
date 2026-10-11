SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO Admission (Reg_No, Is_Admitted)
    VALUES ('TSTIPD001', 1);
    DECLARE @AdmissionID bigint = SCOPE_IDENTITY();

    INSERT INTO Money_Receipt (Reg_No, Against, Amount, Trans_Date, Entered_By)
    VALUES ('TSTIPD001', 'Advance', 100.00, GETDATE(), 1);
    DECLARE @ActiveReceiptID bigint = SCOPE_IDENTITY();

    INSERT INTO Money_Receipt (Reg_No, Against, Amount, Trans_Date, Entered_By)
    VALUES ('TSTIPD001', 'Advance', 40.00, GETDATE(), 1);
    DECLARE @ReversedReceiptID bigint = SCOPE_IDENTITY();

    INSERT INTO Money_Receipt (Reg_No, Against, Amount, Trans_Date, Entered_By)
    VALUES ('TSTIPD001', 'DoctorPayment', 25.00, GETDATE(), 1);

    INSERT INTO Financial_Reversal
        (Entity_Type, Entity_ID, Amount, Reason, Reversed_By, Reversed_At)
    VALUES
        ('IPD Money Receipt', @ReversedReceiptID, 40.00, 'Rollback smoke test', 1, GETDATE());

    DECLARE @ActiveReceiptTotal decimal(18,2) =
    (
        SELECT COALESCE(SUM(r.Amount), 0)
        FROM Money_Receipt r
        WHERE r.Reg_No = 'TSTIPD001'
          AND r.Against <> 'DoctorPayment'
          AND NOT EXISTS
          (
              SELECT 1
              FROM Financial_Reversal f
              WHERE f.Entity_Type = 'IPD Money Receipt'
                AND f.Entity_ID = r.ID
          )
    );

    IF @ActiveReceiptTotal <> 100.00
        THROW 51000, 'Reversed or doctor receipt exclusion failed.', 1;

    INSERT INTO IPD_Final_Bill
    (
        Admission_ID, Reg_No, Admission_Fee, Bed_Charges, Service_Charges,
        Hospital_Charges, Ambulance_Charges, Gross_Amount, Discount_Amount,
        Net_Amount, Receipt_Amount, Applied_Advance_Amount, Balance_Due,
        Excess_Payment, Settlement_Status, Discharge_Date, Outcome,
        Generated_By, Generated_At
    )
    VALUES
    (
        @AdmissionID, 'TSTIPD001', 100, 500, 300, 100, 0, 1000, 100,
        900, @ActiveReceiptTotal, 100, 800, 0, 'Due', GETDATE(), 'Cured',
        1, GETDATE()
    );
    DECLARE @BillNo bigint = SCOPE_IDENTITY();

    INSERT INTO IPD_Final_Bill_Receipt
        (Bill_No, Money_Receipt_ID, Amount, Against)
    VALUES
        (@BillNo, @ActiveReceiptID, 100.00, 'Advance');

    IF NOT EXISTS
    (
        SELECT 1
        FROM IPD_Final_Bill
        WHERE Bill_No = @BillNo
          AND Net_Amount = 900.00
          AND Receipt_Amount = 100.00
          AND Balance_Due = 800.00
    )
        THROW 51001, 'Final bill snapshot persistence failed.', 1;

    IF (SELECT COUNT(*) FROM IPD_Final_Bill_Receipt WHERE Bill_No = @BillNo) <> 1
        THROW 51002, 'Final bill receipt allocation failed.', 1;

    ROLLBACK TRANSACTION;
    SELECT 'PASS' AS Result, @ActiveReceiptTotal AS ActiveReceiptTotal, @BillNo AS GeneratedBillNo;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
