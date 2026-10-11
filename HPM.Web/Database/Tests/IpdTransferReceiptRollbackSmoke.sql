SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @Now datetime = GETDATE();
    DECLARE @RegNo nvarchar(10) = N'UAT-IPD-01';
    DECLARE @SourceBed int;
    DECLARE @TargetBed int;
    DECLARE @PatientTransactionID bigint;
    DECLARE @MoneyReceiptID bigint;

    INSERT INTO Bed_Cabin (Name, Category_ID, Is_Allocated) VALUES (N'UAT-SOURCE', 1, 1);
    SET @SourceBed = SCOPE_IDENTITY();
    INSERT INTO Bed_Cabin (Name, Category_ID, Is_Allocated) VALUES (N'UAT-TARGET', 1, 0);
    SET @TargetBed = SCOPE_IDENTITY();

    INSERT INTO Admission (Reg_No, Bed_Cabin_ID, Admission_Date, Bed_Admission_Date, Is_Admitted, Is_Package, Entered_By, Trans_Date)
    VALUES (@RegNo, @SourceBed, DATEADD(day, -1, @Now), DATEADD(day, -1, @Now), 1, 0, 1, @Now);

    INSERT INTO Bed_Cabin_Transection (Bed_Cabin_ID, Reg_No, Rate, Start_Date)
    VALUES (@SourceBed, @RegNo, 1000, DATEADD(day, -1, @Now));

    INSERT INTO Patient_Transaction (Reg_No, Service_ID, Service_Rate, Quantity, IPD_Dept_ID, Entered_By, Trans_Date)
    VALUES (@RegNo, @TargetBed, 1000, 0, 1, 1, @Now);
    SET @PatientTransactionID = SCOPE_IDENTITY();

    UPDATE Bed_Cabin_Transection SET End_Date = DATEADD(minute, -1, @Now) WHERE Reg_No = @RegNo AND End_Date IS NULL;
    UPDATE Bed_Cabin SET Is_Allocated = 0 WHERE ID = @SourceBed;
    UPDATE Bed_Cabin SET Is_Allocated = 1 WHERE ID = @TargetBed;
    UPDATE Admission SET Bed_Cabin_ID = @TargetBed, Bed_Admission_Date = @Now WHERE Reg_No = @RegNo;
    INSERT INTO Bed_Cabin_Transection (Bed_Cabin_ID, Reg_No, Rate, Patient_Transaction_ID, Start_Date)
    VALUES (@TargetBed, @RegNo, 1000, @PatientTransactionID, @Now);

    INSERT INTO Money_Receipt (Reg_No, Against, Amount, Trans_Date, Entered_By)
    VALUES (@RegNo, N'Advance', 2500, @Now, 1);
    SET @MoneyReceiptID = SCOPE_IDENTITY();
    INSERT INTO Patient_Payment (MR_ID, Reg_No, Amount, Entered_By, Trans_Date, IS_Hospital_Payment)
    VALUES (@MoneyReceiptID, @RegNo, 2500, 1, @Now, 1);
    INSERT INTO Central_Cash_Collection (Amount, Module_Flag, Head_ID, Entered_By, Trans_Date, Trans_Date_CL, Reference_ID)
    VALUES (2500, 1, 1, 1, @Now, @Now, @MoneyReceiptID);

    IF NOT EXISTS (SELECT 1 FROM Admission WHERE Reg_No = @RegNo AND Bed_Cabin_ID = @TargetBed)
        THROW 51000, 'Admission bed transfer invariant failed.', 1;
    IF NOT EXISTS (SELECT 1 FROM Bed_Cabin_Transection WHERE Reg_No = @RegNo AND Bed_Cabin_ID = @TargetBed AND Patient_Transaction_ID = @PatientTransactionID AND End_Date IS NULL)
        THROW 51000, 'Bed movement link invariant failed.', 1;
    IF NOT EXISTS (SELECT 1 FROM Patient_Payment WHERE MR_ID = @MoneyReceiptID AND IS_Hospital_Payment = 1)
        THROW 51000, 'Patient payment link invariant failed.', 1;
    IF NOT EXISTS (SELECT 1 FROM Central_Cash_Collection WHERE Reference_ID = @MoneyReceiptID AND Module_Flag = 1 AND Head_ID = 1)
        THROW 51000, 'Central cash link invariant failed.', 1;

    SELECT N'PASS' AS Result, @RegNo AS Reg_No, @PatientTransactionID AS Patient_Transaction_ID, @MoneyReceiptID AS Money_Receipt_ID;
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

IF EXISTS (SELECT 1 FROM Admission WHERE Reg_No = N'UAT-IPD-01')
    THROW 51000, 'Rollback verification failed.', 1;

SELECT N'PASS' AS Rollback_Result;
